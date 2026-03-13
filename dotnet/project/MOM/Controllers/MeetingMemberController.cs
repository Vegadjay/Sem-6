using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingMemberController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MeetingMemberController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }

        public async Task<IActionResult> Index()
        {
            var list = new List<MeetingMemberModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    SELECT mm.MeetingMemberID, mm.MeetingID, mm.StaffID, mm.IsPresent, mm.Remarks,
                           m.MeetingDate, m.MeetingDescription,
                           s.StaffName
                    FROM MOM_MeetingMember mm
                    LEFT JOIN MOM_Meetings m ON mm.MeetingID = m.MeetingID
                    LEFT JOIN MOM_Staff s ON mm.StaffID = s.StaffID
                    ORDER BY m.MeetingDate DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new MeetingMemberModel
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
                                    MeetingDescription = reader.IsDBNull(reader.GetOrdinal("MeetingDescription")) ? null : reader.GetString(reader.GetOrdinal("MeetingDescription"))
                                },
                                Staff = reader.IsDBNull(reader.GetOrdinal("StaffID")) ? null : new StaffModel
                                {
                                    StaffID = reader.GetInt32(reader.GetOrdinal("StaffID")),
                                    StaffName = reader.GetString(reader.GetOrdinal("StaffName"))
                                }
                            });
                        }
                    }
                }
            }

            return View("MeetingMemberList", list);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View("MeetingMemberAddEdit", new MeetingMemberModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingMemberAddEdit", model);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingMember_Insert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MeetingID", (object?)model.MeetingID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StaffID", (object?)model.StaffID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsPresent", model.IsPresent);
                    command.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            MeetingMemberModel? model = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM MOM_MeetingMember WHERE MeetingMemberID = @MeetingMemberID", connection))
                {
                    command.Parameters.AddWithValue("@MeetingMemberID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model = new MeetingMemberModel
                            {
                                MeetingMemberID = reader.GetInt32(reader.GetOrdinal("MeetingMemberID")),
                                MeetingID = reader.IsDBNull(reader.GetOrdinal("MeetingID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                StaffID = reader.IsDBNull(reader.GetOrdinal("StaffID")) ? null : reader.GetInt32(reader.GetOrdinal("StaffID")),
                                IsPresent = reader.GetBoolean(reader.GetOrdinal("IsPresent")),
                                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks"))
                            };
                        }
                    }
                }
            }
            
            if (model == null) return NotFound();

            await LoadDropdowns();
            return View("MeetingMemberAddEdit", model);
        }

        [HttpPost]
 
        public async Task<IActionResult> Update(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingMemberAddEdit", model);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingMember_UpdateByPK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MeetingMemberID", model.MeetingMemberID);
                    command.Parameters.AddWithValue("@MeetingID", (object?)model.MeetingID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StaffID", (object?)model.StaffID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsPresent", model.IsPresent);
                    command.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);

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
                    using (var command = new SqlCommand("PR_MOM_MeetingMember_DeleteByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MeetingMemberID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                TempData["Success"] = "Attendance record deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this record because it is referenced by other data.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the record.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var meetings = new List<dynamic>();
            var staffList = new List<StaffModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                // Load Meetings
                using (var command = new SqlCommand("SELECT MeetingID, MeetingDate, MeetingDescription FROM MOM_Meetings ORDER BY MeetingDate DESC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            meetings.Add(new 
                            { 
                                MeetingID = reader.GetInt32(reader.GetOrdinal("MeetingID")), 
                                MeetingDate = reader.IsDBNull(reader.GetOrdinal("MeetingDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("MeetingDate")),
                                MeetingDescription = reader.IsDBNull(reader.GetOrdinal("MeetingDescription")) ? null : reader.GetString(reader.GetOrdinal("MeetingDescription"))
                            });
                        }
                    }
                }

                // Load Staff
                using (var command = new SqlCommand("SELECT StaffID, StaffName FROM MOM_Staff ORDER BY StaffName", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            staffList.Add(new StaffModel
                            {
                                StaffID = reader.GetInt32(reader.GetOrdinal("StaffID")),
                                StaffName = reader.GetString(reader.GetOrdinal("StaffName"))
                            });
                        }
                    }
                }
            }

            ViewBag.MeetingList = meetings.Select(m => new { 
                MeetingID = m.MeetingID, 
                DisplayName = (m.MeetingDate?.ToString("dd MMM yyyy") ?? "N/A") + " - " + m.MeetingDescription 
            }).ToList();

            ViewBag.StaffList = staffList;
        }
    }
}
