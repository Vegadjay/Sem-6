using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MeetingsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }

        public async Task<IActionResult> Index(
            int? MeetingTypeID = null,
            int? DepartmentID = null,
            int? MeetingVenueID = null,
            DateTime? StartDate = null,
            DateTime? EndDate = null,
            string? SearchText = null,
            int? page = null)
        {
            await LoadDropdowns();

            // Set current filter values for the view
            ViewBag.CurrentMeetingTypeID = MeetingTypeID;
            ViewBag.CurrentDepartmentID = DepartmentID;
            ViewBag.CurrentMeetingVenueID = MeetingVenueID;
            ViewBag.CurrentStartDate = StartDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = EndDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentSearchText = SearchText;

            var meetings = new List<MeetingListVM>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand("PR_MOM_Search_Meetings", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MeetingTypeID", (object?)MeetingTypeID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DepartmentID", (object?)DepartmentID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingVenueID", (object?)MeetingVenueID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StartDate", (object?)StartDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", (object?)EndDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SearchText", (object?)SearchText ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            meetings.Add(new MeetingListVM
                            {
                                MeetingID = reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                MeetingDate = reader.IsDBNull(reader.GetOrdinal("MeetingDate")) ? null : reader.GetDateTime(reader.GetOrdinal("MeetingDate")),
                                MeetingVenueName = reader.IsDBNull(reader.GetOrdinal("MeetingVenueName")) ? null : reader.GetString(reader.GetOrdinal("MeetingVenueName")),
                                MeetingTypeName = reader.IsDBNull(reader.GetOrdinal("MeetingTypeName")) ? null : reader.GetString(reader.GetOrdinal("MeetingTypeName")),
                                DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? null : reader.GetString(reader.GetOrdinal("DepartmentName")),
                                MeetingDescription = reader.IsDBNull(reader.GetOrdinal("MeetingDescription")) ? null : reader.GetString(reader.GetOrdinal("MeetingDescription")),
                                IsCancelled = reader.IsDBNull(reader.GetOrdinal("IsCancelled")) ? null : reader.GetBoolean(reader.GetOrdinal("IsCancelled"))
                            });
                        }
                    }
                }
            }

            // Pagination logic
            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = meetings.Count;
            var pagedList = meetings.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View("MeetingList", pagedList);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View("MeetingAddEdit", new MeetingsModel 
            { 
                MeetingDate = DateTime.Now.AddMinutes(1).AddSeconds(-DateTime.Now.Second) // Set to current time rounded to next minute
            });
        }

        [HttpPost]
 
        public async Task<IActionResult> Create(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingAddEdit", model);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_Meetings_Insert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MeetingDate", (object?)model.MeetingDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingVenueID", (object?)model.MeetingVenueID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingTypeID", (object?)model.MeetingTypeID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DepartmentID", (object?)model.DepartmentID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingDescription", (object?)model.MeetingDescription ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DocumentPath", (object?)model.DocumentPath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsCancelled", false);
                    command.Parameters.AddWithValue("@CancellationDateTime", DBNull.Value);
                    command.Parameters.AddWithValue("@CancellationReason", DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            MeetingsModel? meeting = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // 1. Fetch meeting and related lookup tables
                string meetingQuery = @"
                    SELECT m.*, 
                           mt.MeetingTypeName, 
                           d.DepartmentName, 
                           mv.MeetingVenueName
                    FROM MOM_Meetings m
                    LEFT JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
                    LEFT JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
                    LEFT JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
                    WHERE m.MeetingID = @MeetingID";

                using (var command = new SqlCommand(meetingQuery, connection))
                {
                    command.Parameters.AddWithValue("@MeetingID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            meeting = new MeetingsModel
                            {
                                MeetingID = reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                MeetingDate = reader.IsDBNull(reader.GetOrdinal("MeetingDate")) ? null : reader.GetDateTime(reader.GetOrdinal("MeetingDate")),
                                MeetingVenueID = reader.IsDBNull(reader.GetOrdinal("MeetingVenueID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                MeetingTypeID = reader.IsDBNull(reader.GetOrdinal("MeetingTypeID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                DepartmentID = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                MeetingDescription = reader.IsDBNull(reader.GetOrdinal("MeetingDescription")) ? null : reader.GetString(reader.GetOrdinal("MeetingDescription")),
                                DocumentPath = reader.IsDBNull(reader.GetOrdinal("DocumentPath")) ? null : reader.GetString(reader.GetOrdinal("DocumentPath")),
                                IsCancelled = reader.IsDBNull(reader.GetOrdinal("IsCancelled")) ? null : reader.GetBoolean(reader.GetOrdinal("IsCancelled")),
                                CancellationDateTime = reader.IsDBNull(reader.GetOrdinal("CancellationDateTime")) ? null : reader.GetDateTime(reader.GetOrdinal("CancellationDateTime")),
                                CancellationReason = reader.IsDBNull(reader.GetOrdinal("CancellationReason")) ? null : reader.GetString(reader.GetOrdinal("CancellationReason")),
                                MeetingType = reader.IsDBNull(reader.GetOrdinal("MeetingTypeID")) ? null : new MeetingTypeModel 
                                {
                                    MeetingTypeID = reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                    MeetingTypeName = reader.IsDBNull(reader.GetOrdinal("MeetingTypeName")) ? null : reader.GetString(reader.GetOrdinal("MeetingTypeName"))
                                },
                                Department = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : new DepartmentModel
                                {
                                    DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                    DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? null : reader.GetString(reader.GetOrdinal("DepartmentName"))
                                },
                                MeetingVenue = reader.IsDBNull(reader.GetOrdinal("MeetingVenueID")) ? null : new MeetingVenueModel
                                {
                                    MeetingVenueID = reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                    MeetingVenueName = reader.IsDBNull(reader.GetOrdinal("MeetingVenueName")) ? null : reader.GetString(reader.GetOrdinal("MeetingVenueName"))
                                }
                            };
                        }
                    }
                }

                if (meeting == null) return NotFound();

                // 2. Fetch MeetingMembers with Staff and Department
                meeting.MeetingMembers = new List<MeetingMemberModel>();
                var currentMemberIds = new List<int?>();

                string membersQuery = @"
                    SELECT mm.*, s.StaffName, d.DepartmentID, d.DepartmentName
                    FROM MOM_MeetingMember mm
                    INNER JOIN MOM_Staff s ON mm.StaffID = s.StaffID
                    LEFT JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
                    WHERE mm.MeetingID = @MeetingID";
                    
                using (var command = new SqlCommand(membersQuery, connection))
                {
                    command.Parameters.AddWithValue("@MeetingID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int? staffId = reader.IsDBNull(reader.GetOrdinal("StaffID")) ? null : reader.GetInt32(reader.GetOrdinal("StaffID"));
                            currentMemberIds.Add(staffId);

                            meeting.MeetingMembers.Add(new MeetingMemberModel
                            {
                                MeetingMemberID = reader.GetInt32(reader.GetOrdinal("MeetingMemberID")),
                                MeetingID = reader.IsDBNull(reader.GetOrdinal("MeetingID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                StaffID = staffId,
                                IsPresent = reader.GetBoolean(reader.GetOrdinal("IsPresent")),
                                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                                Staff = new StaffModel
                                {
                                    StaffID = reader.GetInt32(reader.GetOrdinal("StaffID")),
                                    StaffName = reader.GetString(reader.GetOrdinal("StaffName")),
                                    Department = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : new DepartmentModel
                                    {
                                        DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                        DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                    }
                                }
                            });
                        }
                    }
                }

                // 3. Fetch StaffList for Dropdown (excluding existing members)
                var staffList = new List<StaffModel>();
                string staffQuery = "SELECT s.*, d.DepartmentName FROM MOM_Staff s LEFT JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID";
                
                using (var command = new SqlCommand(staffQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int currentStaffId = reader.GetInt32(reader.GetOrdinal("StaffID"));
                            if (!currentMemberIds.Contains(currentStaffId))
                            {
                                staffList.Add(new StaffModel
                                {
                                    StaffID = currentStaffId,
                                    StaffName = reader.GetString(reader.GetOrdinal("StaffName")),
                                    Department = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : new DepartmentModel
                                    {
                                        DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                        DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                    }
                                });
                            }
                        }
                    }
                }
                ViewBag.StaffList = staffList;
            }

            return View(meeting);
        }

        [HttpPost]
 
        public async Task<IActionResult> AddMember(int meetingId, int staffId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingMember_Insert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MeetingID", meetingId);
                    command.Parameters.AddWithValue("@StaffID", staffId);
                    command.Parameters.AddWithValue("@IsPresent", false);
                    command.Parameters.AddWithValue("@Remarks", "");

                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        [HttpPost]
 
        public async Task<IActionResult> RemoveMember(int meetingMemberId, int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DELETE FROM MOM_MeetingMember WHERE MeetingMemberID = @MeetingMemberID", connection))
                {
                    command.Parameters.AddWithValue("@MeetingMemberID", meetingMemberId);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        public async Task<IActionResult> Update(int id)
        {
            MeetingsModel? meeting = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM MOM_Meetings WHERE MeetingID = @MeetingID", connection))
                {
                    command.Parameters.AddWithValue("@MeetingID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            meeting = new MeetingsModel
                            {
                                MeetingID = reader.GetInt32(reader.GetOrdinal("MeetingID")),
                                MeetingDate = reader.IsDBNull(reader.GetOrdinal("MeetingDate")) ? null : reader.GetDateTime(reader.GetOrdinal("MeetingDate")),
                                MeetingVenueID = reader.IsDBNull(reader.GetOrdinal("MeetingVenueID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                MeetingTypeID = reader.IsDBNull(reader.GetOrdinal("MeetingTypeID")) ? null : reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                DepartmentID = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                MeetingDescription = reader.IsDBNull(reader.GetOrdinal("MeetingDescription")) ? null : reader.GetString(reader.GetOrdinal("MeetingDescription")),
                                DocumentPath = reader.IsDBNull(reader.GetOrdinal("DocumentPath")) ? null : reader.GetString(reader.GetOrdinal("DocumentPath")),
                                IsCancelled = reader.IsDBNull(reader.GetOrdinal("IsCancelled")) ? null : reader.GetBoolean(reader.GetOrdinal("IsCancelled")),
                                CancellationDateTime = reader.IsDBNull(reader.GetOrdinal("CancellationDateTime")) ? null : reader.GetDateTime(reader.GetOrdinal("CancellationDateTime")),
                                CancellationReason = reader.IsDBNull(reader.GetOrdinal("CancellationReason")) ? null : reader.GetString(reader.GetOrdinal("CancellationReason"))
                            };
                        }
                    }
                }
            }

            if (meeting == null) return NotFound();

            await LoadDropdowns();
            return View("MeetingAddEdit", meeting);
        }

        [HttpPost]
 
        public async Task<IActionResult> Update(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingAddEdit", model);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_Meetings_UpdateByPK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MeetingID", model.MeetingID);
                    command.Parameters.AddWithValue("@MeetingDate", (object?)model.MeetingDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingVenueID", (object?)model.MeetingVenueID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingTypeID", (object?)model.MeetingTypeID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DepartmentID", (object?)model.DepartmentID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MeetingDescription", (object?)model.MeetingDescription ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DocumentPath", (object?)model.DocumentPath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsCancelled", (object?)model.IsCancelled ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CancellationDateTime", (object?)model.CancellationDateTime ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CancellationReason", (object?)model.CancellationReason ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
 
        public async Task<IActionResult> UpdateAttendance(List<MeetingMemberModel> members, int meetingId)
        {
            if (members != null && members.Any())
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    foreach (var member in members)
                    {
                        using (var command = new SqlCommand("UPDATE MeetingMembers SET IsPresent = @IsPresent, Modified = GETDATE() WHERE MeetingMemberID = @MeetingMemberID", connection))
                        {
                            command.Parameters.AddWithValue("@IsPresent", member.IsPresent);
                            command.Parameters.AddWithValue("@MeetingMemberID", member.MeetingMemberID);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        [HttpPost]
 
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("PR_MOM_Meetings_DeleteByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MeetingID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                TempData["Success"] = "Meeting deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the meeting.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var meetingTypes = new List<MeetingTypeModel>();
            var meetingVenues = new List<MeetingVenueModel>();
            var departments = new List<DepartmentModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Types
                using (var command = new SqlCommand("PR_MOM_MeetingType_SelectAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            meetingTypes.Add(new MeetingTypeModel
                            {
                                MeetingTypeID = reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                MeetingTypeName = reader.GetString(reader.GetOrdinal("MeetingTypeName"))
                            });
                        }
                    }
                }

                // Venues
                using (var command = new SqlCommand("PR_MOM_MeetingVenue_SelectAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            meetingVenues.Add(new MeetingVenueModel
                            {
                                MeetingVenueID = reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                MeetingVenueName = reader.GetString(reader.GetOrdinal("MeetingVenueName"))
                            });
                        }
                    }
                }

                // Departments
                using (var command = new SqlCommand("PR_MOM_Department_SelectAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
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

            ViewBag.MeetingTypeList = meetingTypes;
            ViewBag.MeetingVenueList = meetingVenues;
            ViewBag.DepartmentList = departments;
        }
    }
}
