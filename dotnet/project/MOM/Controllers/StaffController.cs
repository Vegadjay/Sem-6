using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public StaffController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }

        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            // Set current search text for the view
            ViewBag.CurrentSearchText = SearchText;

            var allStaff = new List<StaffListVM>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand("PR_MOM_Staff_SelectAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allStaff.Add(new StaffListVM
                            {
                                StaffID = reader.GetInt32(reader.GetOrdinal("StaffID")),
                                StaffName = reader.GetString(reader.GetOrdinal("StaffName")),
                                DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? null : reader.GetString(reader.GetOrdinal("DepartmentName")),
                                EmailAddress = reader.IsDBNull(reader.GetOrdinal("EmailAddress")) ? null : reader.GetString(reader.GetOrdinal("EmailAddress")),
                                MobileNo = reader.IsDBNull(reader.GetOrdinal("MobileNo")) ? null : reader.GetString(reader.GetOrdinal("MobileNo"))
                            });
                        }
                    }
                }

                // apply search filter in-memory if needed (since the stored procedure doesn't have search)
                if (!string.IsNullOrEmpty(SearchText))
                {
                    var lowerSearch = SearchText.ToLower();
                    allStaff = allStaff.Where(s =>
                        (s.StaffName?.ToLower().Contains(lowerSearch) ?? false) ||
                        (s.DepartmentName?.ToLower().Contains(lowerSearch) ?? false) ||
                        (s.EmailAddress?.ToLower().Contains(lowerSearch) ?? false) ||
                        (s.MobileNo?.ToLower().Contains(lowerSearch) ?? false)
                    ).ToList();
                }

                // Pagination setup
                int pageSize = 10;
                int pageNumber = page ?? 1;
                int totalRecords = allStaff.Count;
                var pagedStaff = allStaff.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                // Efficiently fetch and apply meeting statistics for paged staff
                if (pagedStaff.Any())
                {
                    var staffIds = string.Join(",", pagedStaff.Select(s => s.StaffID));
                    var statsQuery = $"SELECT StaffID, COUNT(*) AS Total, SUM(CASE WHEN IsPresent = 1 THEN 1 ELSE 0 END) AS Present FROM MOM_MeetingMember WHERE StaffID IN ({staffIds}) GROUP BY StaffID";

                    using (var statsCommand = new SqlCommand(statsQuery, connection))
                    {
                        using (var reader = await statsCommand.ExecuteReaderAsync())
                        {
                            var statsDict = new Dictionary<int, (int Total, int Present)>();
                            while (await reader.ReadAsync())
                            {
                                statsDict.Add(
                                    reader.GetInt32(reader.GetOrdinal("StaffID")),
                                    (reader.GetInt32(reader.GetOrdinal("Total")), reader.GetInt32(reader.GetOrdinal("Present")))
                                );
                            }

                            foreach (var s in pagedStaff)
                            {
                                if (statsDict.TryGetValue(s.StaffID, out var stat))
                                {
                                    s.TotalMeetings = stat.Total;
                                    s.AttendanceRate = stat.Total > 0 ? (double)stat.Present / stat.Total * 100 : 0;
                                }
                                else
                                {
                                    s.TotalMeetings = 0;
                                    s.AttendanceRate = 0;
                                }
                            }
                        }
                    }
                }

                // Pagination view bags
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageSize = pageSize;

                return View("StaffList", pagedStaff);
            }
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View("StaffAddEdit", new StaffModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(StaffModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("StaffAddEdit", model);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string insertQuery = @"
                    INSERT INTO MOM_Staff (StaffName, DepartmentID, MobileNo, EmailAddress)
                    VALUES (@StaffName, @DepartmentID, @MobileNo, @EmailAddress)";
                
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@StaffName", model.StaffName);
                    command.Parameters.AddWithValue("@DepartmentID", (object?)model.DepartmentID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MobileNo", (object?)model.MobileNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmailAddress", (object?)model.EmailAddress ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            StaffModel? staff = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM MOM_Staff WHERE StaffID = @StaffID", connection))
                {
                    command.Parameters.AddWithValue("@StaffID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            staff = new StaffModel
                            {
                                StaffID = reader.GetInt32(reader.GetOrdinal("StaffID")),
                                StaffName = reader.GetString(reader.GetOrdinal("StaffName")),
                                DepartmentID = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                MobileNo = reader.IsDBNull(reader.GetOrdinal("MobileNo")) ? null : reader.GetString(reader.GetOrdinal("MobileNo")),
                                EmailAddress = reader.IsDBNull(reader.GetOrdinal("EmailAddress")) ? null : reader.GetString(reader.GetOrdinal("EmailAddress"))
                            };
                        }
                    }
                }
            }

            if (staff == null) return NotFound();

            await LoadDropdowns();
            return View("StaffAddEdit", staff);
        }

        [HttpPost]
        public async Task<IActionResult> Update(StaffModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("StaffAddEdit", model);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string updateQuery = @"
                    UPDATE MOM_Staff 
                    SET StaffName = @StaffName, DepartmentID = @DepartmentID, MobileNo = @MobileNo, EmailAddress = @EmailAddress 
                    WHERE StaffID = @StaffID";
                
                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@StaffID", model.StaffID);
                    command.Parameters.AddWithValue("@StaffName", model.StaffName);
                    command.Parameters.AddWithValue("@DepartmentID", (object?)model.DepartmentID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MobileNo", (object?)model.MobileNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmailAddress", (object?)model.EmailAddress ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
 
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM MOM_Staff WHERE StaffID = @StaffID", connection))
                    {
                        command.Parameters.AddWithValue("@StaffID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                TempData["Success"] = "Staff member deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete staff member as they are linked to existing meeting records.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the staff member.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            StaffModel? staff = null;
            var staffMeetings = new List<MeetingMemberModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // 1. Fetch Staff with Department
                string staffQuery = @"
                    SELECT s.*, d.DepartmentName 
                    FROM MOM_Staff s
                    LEFT JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
                    WHERE s.StaffID = @StaffID";

                using (var command = new SqlCommand(staffQuery, connection))
                {
                    command.Parameters.AddWithValue("@StaffID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            staff = new StaffModel
                            {
                                StaffID = reader.GetInt32(reader.GetOrdinal("StaffID")),
                                StaffName = reader.GetString(reader.GetOrdinal("StaffName")),
                                DepartmentID = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                MobileNo = reader.IsDBNull(reader.GetOrdinal("MobileNo")) ? null : reader.GetString(reader.GetOrdinal("MobileNo")),
                                EmailAddress = reader.IsDBNull(reader.GetOrdinal("EmailAddress")) ? null : reader.GetString(reader.GetOrdinal("EmailAddress")),
                                Department = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : new DepartmentModel
                                {
                                    DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                }
                            };
                        }
                    }
                }

                if (staff == null) return NotFound();

                // 2. Fetch recent meetings for this staff member
                string meetingsQuery = @"
                    SELECT mm.MeetingMemberID, mm.MeetingID, mm.StaffID, mm.IsPresent, mm.Remarks,
                           m.MeetingDate, m.MeetingDescription, 
                           mt.MeetingTypeID, mt.MeetingTypeName, 
                           mv.MeetingVenueID, mv.MeetingVenueName
                    FROM MOM_MeetingMember mm
                    INNER JOIN MOM_Meetings m ON mm.MeetingID = m.MeetingID
                    LEFT JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
                    LEFT JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
                    WHERE mm.StaffID = @StaffID
                    ORDER BY m.MeetingDate DESC";

                using (var command = new SqlCommand(meetingsQuery, connection))
                {
                    command.Parameters.AddWithValue("@StaffID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            staffMeetings.Add(new MeetingMemberModel
                            {
                                MeetingMemberID = reader.GetInt32(reader.GetOrdinal("MeetingMemberID")),
                                MeetingID = reader.IsDBNull(reader.GetOrdinal("MeetingID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                StaffID = reader.IsDBNull(reader.GetOrdinal("StaffID")) ? null : reader.GetInt32(reader.GetOrdinal("StaffID")),
                                IsPresent = reader.GetBoolean(reader.GetOrdinal("IsPresent")),
                                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                                Meeting = reader.IsDBNull(reader.GetOrdinal("MeetingID")) ? null : new MeetingsModel
                                {
                                    MeetingID = reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                    MeetingDate = reader.IsDBNull(reader.GetOrdinal("MeetingDate")) ? null : reader.GetDateTime(reader.GetOrdinal("MeetingDate")),
                                    MeetingDescription = reader.IsDBNull(reader.GetOrdinal("MeetingDescription")) ? null : reader.GetString(reader.GetOrdinal("MeetingDescription")),
                                    MeetingType = reader.IsDBNull(reader.GetOrdinal("MeetingTypeID")) ? null : new MeetingTypeModel 
                                    {
                                        MeetingTypeID = reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                        MeetingTypeName = reader.IsDBNull(reader.GetOrdinal("MeetingTypeName")) ? null : reader.GetString(reader.GetOrdinal("MeetingTypeName"))
                                    },
                                    MeetingVenue = reader.IsDBNull(reader.GetOrdinal("MeetingVenueID")) ? null : new MeetingVenueModel
                                    {
                                        MeetingVenueID = reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                        MeetingVenueName = reader.IsDBNull(reader.GetOrdinal("MeetingVenueName")) ? null : reader.GetString(reader.GetOrdinal("MeetingVenueName"))
                                    }
                                }
                            });
                        }
                    }
                }
            }

            ViewBag.StaffMeetings = staffMeetings;
            return View(staff);
        }

        private async Task LoadDropdowns()
        {
            var departments = new List<DepartmentModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT DepartmentID, DepartmentName FROM MOM_Department ORDER BY DepartmentName", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            departments.Add(new DepartmentModel
                            {
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            });
                        }
                    }
                }
            }
            ViewBag.DepartmentList = departments;
        }
    }
}
