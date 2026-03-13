using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MOMConnection") ?? throw new InvalidOperationException("Connection string 'MOMConnection' not found.");
        }

        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            ViewBag.CurrentSearchText = SearchText;

            var departments = new List<DepartmentModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_Department_Search", connection))
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
                            departments.Add(new DepartmentModel
                            {
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            });
                        }
                    }
                }
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = departments.Count;
            var pagedList = departments.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View("DepartmentList", pagedList);
        }

        public IActionResult Create() => View("DepartmentAddEdit", new DepartmentModel());

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentModel model)
        {
            if (!ModelState.IsValid) return View("DepartmentAddEdit", model);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_Department_Insert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            DepartmentModel? department = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT DepartmentID, DepartmentName FROM MOM_Department WHERE DepartmentID = @DepartmentID", connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            department = new DepartmentModel
                            {
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            };
                        }
                    }
                }
            }

            if (department == null) return NotFound();
            return View("DepartmentAddEdit", department);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DepartmentModel model)
        {
            if (!ModelState.IsValid) return View("DepartmentAddEdit", model);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PR_MOM_Department_UpdateByPK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                    command.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                    await command.ExecuteNonQueryAsync();
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            DepartmentModel? department = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT DepartmentID, DepartmentName FROM MOM_Department WHERE DepartmentID = @DepartmentID", connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            department = new DepartmentModel
                            {
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            };
                        }
                    }
                }
            }

            if (department == null) return NotFound();
            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("PR_MOM_Department_DeleteByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DepartmentID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                TempData["Success"] = "Department deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete department as it is referenced by existing staff members or meetings.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the department.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
