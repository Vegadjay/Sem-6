using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            ViewBag.CurrentSearchText = SearchText;

            // Use search stored procedure via EF
            var searchTextParam = new SqlParameter("@SearchText", (object?)SearchText ?? DBNull.Value);
            var departments = await _context.Departments
                .FromSqlRaw("EXEC PR_MOM_Department_Search @SearchText", searchTextParam)
                .ToListAsync();

            // Pagination setup
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentModel model)
        {
            if (!ModelState.IsValid) return View("DepartmentAddEdit", model);

            var parameters = new[]
            {
                new SqlParameter("@DepartmentName", model.DepartmentName)
            };
            await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_Department_Insert @DepartmentName", parameters);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            return View("DepartmentAddEdit", department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DepartmentModel model)
        {
            if (!ModelState.IsValid) return View("DepartmentAddEdit", model);

            var parameters = new[]
            {
                new SqlParameter("@DepartmentID", model.DepartmentID),
                new SqlParameter("@DepartmentName", model.DepartmentName)
            };
            await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_Department_UpdateByPK @DepartmentID, @DepartmentName", parameters);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null) return NotFound();
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_Department_DeleteByPK {0}", id);
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
