using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingVenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? SearchText = null, int? page = null)
        {
            ViewBag.CurrentSearchText = SearchText;

            // Use search stored procedure via EF
            var searchTextParam = new SqlParameter("@SearchText", (object?)SearchText ?? DBNull.Value);
            var data = await _context.MeetingVenues
                .FromSqlRaw("EXEC PR_MOM_MeetingVenue_Search @SearchText", searchTextParam)
                .ToListAsync();

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeetingVenueModel model)
        {
            if (!ModelState.IsValid) return View("MeetingVenueAddEdit", model);

            var parameters = new[]
            {
                new SqlParameter("@VenueName", model.MeetingVenueName)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_MeetingVenue_Insert @VenueName", parameters);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var meetingVenue = await _context.MeetingVenues.FirstOrDefaultAsync(m => m.MeetingVenueID == id);
            if (meetingVenue == null) return NotFound();
            return View(meetingVenue);
        }

        public async Task<IActionResult> Update(int id)
        {
            var meetingVenue = await _context.MeetingVenues.FindAsync(id);
            if (meetingVenue == null) return NotFound();
            return View("MeetingVenueAddEdit", meetingVenue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MeetingVenueModel model)
        {
            if (!ModelState.IsValid) return View("MeetingVenueAddEdit", model);

            var parameters = new[]
            {
                new SqlParameter("@VenueID", model.MeetingVenueID),
                new SqlParameter("@VenueName", model.MeetingVenueName)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_MeetingVenue_UpdateByPK @VenueID, @VenueName", parameters);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_MeetingVenue_DeleteByPK {0}", id);
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
