using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            // Set current search text for the view
            ViewBag.CurrentSearchText = SearchText;

            // Use EF Core for searching and fetching staff
            var staffQuery = _context.Set<StaffListVM>()
                .FromSqlRaw("EXEC PR_MOM_Staff_SelectAll");

            // Execute the initial query to get all staff
            var allStaff = await staffQuery.ToListAsync();

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
                var staffIds = pagedStaff.Select(s => s.StaffID).ToList();
                var stats = await _context.MeetingMembers
                    .Where(mm => mm.StaffID.HasValue && staffIds.Contains(mm.StaffID.Value))
                    .GroupBy(mm => mm.StaffID!.Value)
                    .Select(g => new
                    {
                        StaffID = g.Key,
                        Total = g.Count(),
                        Present = g.Count(x => x.IsPresent)
                    })
                    .ToListAsync();

                foreach (var s in pagedStaff)
                {
                    var stat = stats.FirstOrDefault(x => x.StaffID == s.StaffID);
                    s.TotalMeetings = stat?.Total ?? 0;
                    s.AttendanceRate = (s.TotalMeetings ?? 0) > 0 ? (double)(stat?.Present ?? 0) / (s.TotalMeetings ?? 1) * 100 : 0;
                }
            }

            // Pagination view bags
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View("StaffList", pagedStaff);
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

            _context.Staff.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
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

            _context.Staff.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var staff = await _context.Staff.FindAsync(id);
                if (staff != null)
                {
                    _context.Staff.Remove(staff);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Staff member deleted successfully.";
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
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
            var staff = await _context.Staff
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StaffID == id);

            if (staff == null) return NotFound();

            // Fetch recent meetings for this staff member
            ViewBag.StaffMeetings = await _context.MeetingMembers
                .Include(mm => mm.Meeting).ThenInclude(m => m!.MeetingType)
                .Include(mm => mm.Meeting).ThenInclude(m => m!.MeetingVenue)
                .Where(mm => mm.StaffID == id)
                .OrderByDescending(mm => mm.Meeting!.MeetingDate)
                .ToListAsync();

            return View(staff);
        }

        private async Task LoadDropdowns()
        {
            ViewBag.DepartmentList = await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
        }
    }
}
