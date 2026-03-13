using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MeetingTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }


        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            var data = new List<MeetingTypeModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_MeetingType_Search", connection))
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
                            data.Add(new MeetingTypeModel
                            {
                                MeetingTypeID = reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                MeetingTypeName = reader.GetString(reader.GetOrdinal("MeetingTypeName")),
                                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks"))
                            });
                        }
                    }
                }
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagedList = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentSearchText = SearchText;

            return View("MeetingTypeList", pagedList);
        }


        public IActionResult Create() => View("MeetingTypeAddEdit", new MeetingTypeModel());

        [HttpPost]
 
        public async Task<IActionResult> Create(MeetingTypeModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("PR_MOM_MeetingType_Insert", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MeetingTypeName", model.MeetingTypeName);
                        command.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View("MeetingTypeAddEdit", model);
        }


        public async Task<IActionResult> Update(int id)
        {
            MeetingTypeModel? data = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT MeetingTypeID, MeetingTypeName, Remarks FROM MOM_MeetingType WHERE MeetingTypeID = @MeetingTypeID", connection))
                {
                    command.Parameters.AddWithValue("@MeetingTypeID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            data = new MeetingTypeModel
                            {
                                MeetingTypeID = reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                MeetingTypeName = reader.GetString(reader.GetOrdinal("MeetingTypeName")),
                                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks"))
                            };
                        }
                    }
                }
            }

            if (data == null) return NotFound();
            return View("MeetingTypeAddEdit", data);
        }

        [HttpPost]
 
        public async Task<IActionResult> Update(MeetingTypeModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("PR_MOM_MeetingType_UpdateByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeID);
                        command.Parameters.AddWithValue("@MeetingTypeName", model.MeetingTypeName);
                        command.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View("MeetingTypeAddEdit", model);
        }


        public async Task<IActionResult> Details(int id)
        {
            MeetingTypeModel? meetingType = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT MeetingTypeID, MeetingTypeName, Remarks FROM MOM_MeetingType WHERE MeetingTypeID = @MeetingTypeID", connection))
                {
                    command.Parameters.AddWithValue("@MeetingTypeID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            meetingType = new MeetingTypeModel
                            {
                                MeetingTypeID = reader.GetInt32(reader.GetOrdinal("MeetingTypeID")),
                                MeetingTypeName = reader.GetString(reader.GetOrdinal("MeetingTypeName")),
                                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks"))
                            };
                        }
                    }
                }
            }

            if (meetingType == null)
            {
                return NotFound();
            }

            return View(meetingType);
        }

        [HttpPost]
 
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("PR_MOM_MeetingType_DeleteByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MeetingTypeID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                TempData["Success"] = "Meeting Type deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this Meeting Type because it is used in existing meetings.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the Meeting Type.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
