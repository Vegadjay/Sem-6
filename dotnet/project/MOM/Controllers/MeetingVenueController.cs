using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MeetingVenueController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }

        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            ViewBag.CurrentSearchText = SearchText;

            var data = new List<MeetingVenueModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingVenue_Search", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        command.Parameters.AddWithValue("@SearchText", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@SearchText", SearchText);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            data.Add(new MeetingVenueModel
                            {
                                MeetingVenueID = reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                MeetingVenueName = reader.GetString(reader.GetOrdinal("MeetingVenueName"))
                            });
                        }
                    }
                }
            }

            // Pagination setup
            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = data.Count;
            var pagedList = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View("MeetingVenueList", pagedList);
        }

        public IActionResult Create() => View("MeetingVenueAddEdit", new MeetingVenueModel());

        [HttpPost]
 
        public async Task<IActionResult> Create(MeetingVenueModel model)
        {
            if (!ModelState.IsValid) return View("MeetingVenueAddEdit", model);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingVenue_Insert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VenueName", model.MeetingVenueName);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            MeetingVenueModel? meetingVenue = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT MeetingVenueID, MeetingVenueName FROM MOM_MeetingVenue WHERE MeetingVenueID = @VenueID", connection))
                {
                    command.Parameters.AddWithValue("@VenueID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            meetingVenue = new MeetingVenueModel
                            {
                                MeetingVenueID = reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                MeetingVenueName = reader.GetString(reader.GetOrdinal("MeetingVenueName"))
                            };
                        }
                    }
                }
            }

            if (meetingVenue == null) return NotFound();
            return View(meetingVenue);
        }

        public async Task<IActionResult> Update(int id)
        {
            MeetingVenueModel? meetingVenue = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT MeetingVenueID, MeetingVenueName FROM MOM_MeetingVenue WHERE MeetingVenueID = @VenueID", connection))
                {
                    command.Parameters.AddWithValue("@VenueID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            meetingVenue = new MeetingVenueModel
                            {
                                MeetingVenueID = reader.GetInt32(reader.GetOrdinal("MeetingVenueID")),
                                MeetingVenueName = reader.GetString(reader.GetOrdinal("MeetingVenueName"))
                            };
                        }
                    }
                }
            }

            if (meetingVenue == null) return NotFound();
            return View("MeetingVenueAddEdit", meetingVenue);
        }

        [HttpPost]
 
        public async Task<IActionResult> Update(MeetingVenueModel model)
        {
            if (!ModelState.IsValid) return View("MeetingVenueAddEdit", model);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingVenue_UpdateByPK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VenueID", model.MeetingVenueID);
                    command.Parameters.AddWithValue("@VenueName", model.MeetingVenueName);
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
                    using (var command = new SqlCommand("PR_MOM_MeetingVenue_DeleteByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@VenueID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                TempData["Success"] = "Meeting Venue deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this venue as it is used in existing meetings.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the venue.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
