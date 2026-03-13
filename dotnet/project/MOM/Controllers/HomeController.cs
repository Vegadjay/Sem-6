using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Diagnostics;
using System.Data;

namespace MOM.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return await IndexCoreAsync();
            }
            catch (SqlException ex)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = "Database connection failed. Ensure SQL Server is running and the connection string in appsettings.json is correct. Details: " + ex.Message
                });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = "An error occurred: " + ex.Message });
            }
        }

        private async Task<IActionResult> IndexCoreAsync()
        {
            DashboardViewModel model = new DashboardViewModel();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string countsQuery = @"
                    SELECT 
                        (SELECT COUNT(*) FROM MOM_MeetingType) AS TotalMeetingTypes,
                        (SELECT COUNT(*) FROM MOM_Department) AS TotalDepartments,
                        (SELECT COUNT(*) FROM MOM_Staff) AS TotalStaff,
                        (SELECT COUNT(*) FROM MOM_MeetingVenue) AS TotalVenues,
                        (SELECT COUNT(*) FROM MOM_Meetings) AS TotalMeetings,
                        (SELECT COUNT(*) FROM MOM_Meetings WHERE IsCancelled = 1) AS CancelledMeetings
                ";

                using (var command = new SqlCommand(countsQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model.TotalMeetingTypes = reader.GetInt32(0);
                            model.TotalDepartments = reader.GetInt32(1);
                            model.TotalStaff = reader.GetInt32(2);
                            model.TotalVenues = reader.GetInt32(3);
                            model.TotalMeetings = reader.GetInt32(4);
                            model.CancelledMeetings = reader.GetInt32(5);
                        }
                    }
                }

                // 1. Meetings by Department (Top 5)
                string deptQuery = @"
                    SELECT TOP 5 d.DepartmentName, COUNT(m.MeetingID) AS Count
                    FROM MOM_Meetings m
                    JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
                    GROUP BY d.DepartmentName
                    ORDER BY Count DESC";
                using (var command = new SqlCommand(deptQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        model.DepartmentNames = new List<string>();
                        model.DepartmentCounts = new List<int>();
                        while (await reader.ReadAsync())
                        {
                            model.DepartmentNames.Add(reader.GetString(0));
                            model.DepartmentCounts.Add(reader.GetInt32(1));
                        }
                    }
                }

                // 2. Meetings by Type (Top 5)
                string typeQuery = @"
                    SELECT TOP 5 mt.MeetingTypeName, COUNT(m.MeetingID) AS Count
                    FROM MOM_Meetings m
                    JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
                    GROUP BY mt.MeetingTypeName
                    ORDER BY Count DESC";
                using (var command = new SqlCommand(typeQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        model.MeetingTypeNames = new List<string>();
                        model.MeetingTypeCounts = new List<int>();
                        while (await reader.ReadAsync())
                        {
                            model.MeetingTypeNames.Add(reader.GetString(0));
                            model.MeetingTypeCounts.Add(reader.GetInt32(1));
                        }
                    }
                }

                // 3. Monthly Trends (Last 6 Months)
                string trendQuery = @"
                    SELECT 
                        FORMAT(MeetingDate, 'MMM yyyy') AS MonthLabel, 
                        COUNT(MeetingID) AS Count
                    FROM MOM_Meetings
                    WHERE MeetingDate >= DATEADD(month, -6, GETDATE())
                    GROUP BY FORMAT(MeetingDate, 'MMM yyyy'), YEAR(MeetingDate), MONTH(MeetingDate)
                    ORDER BY YEAR(MeetingDate), MONTH(MeetingDate)";
                using (var command = new SqlCommand(trendQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        model.MonthLabels = new List<string>();
                        model.MonthlyCounts = new List<int>();
                        while (await reader.ReadAsync())
                        {
                            model.MonthLabels.Add(reader.GetString(0));
                            model.MonthlyCounts.Add(reader.GetInt32(1));
                        }
                    }
                }

                // 4. Venue Utilization (Top 5)
                string venueQuery = @"
                    SELECT TOP 5 mv.MeetingVenueName, COUNT(m.MeetingID) AS Count
                    FROM MOM_Meetings m
                    JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
                    GROUP BY mv.MeetingVenueName
                    ORDER BY Count DESC";
                using (var command = new SqlCommand(venueQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        model.VenueNames = new List<string>();
                        model.VenueCounts = new List<int>();
                        while (await reader.ReadAsync())
                        {
                            model.VenueNames.Add(reader.GetString(0));
                            model.VenueCounts.Add(reader.GetInt32(1));
                        }
                    }
                }

                // 5. Top Staff Contributors (Top 5)
                try
                {
                    string staffQuery = @"
                        SELECT TOP 5 s.StaffName, COUNT(mm.MeetingMemberID) AS Count
                        FROM MOM_MeetingMember mm
                        JOIN MOM_Staff s ON mm.StaffID = s.StaffID
                        GROUP BY s.StaffName
                        ORDER BY Count DESC";
                    using (var command = new SqlCommand(staffQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            model.StaffNames = new List<string>();
                            model.StaffCounts = new List<int>();
                            while (await reader.ReadAsync())
                            {
                                model.StaffNames.Add(reader.GetString(0));
                                model.StaffCounts.Add(reader.GetInt32(1));
                            }
                        }
                    }
                }
                catch
                {
                    model.StaffNames = new List<string>();
                    model.StaffCounts = new List<int>();
                }
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
