using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;
using System.Diagnostics;

namespace MOM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                TotalMeetingTypes = await _context.MeetingTypes.CountAsync(),
                TotalDepartments = await _context.Departments.CountAsync(),
                TotalStaff = await _context.Staff.CountAsync(),
                TotalVenues = await _context.MeetingVenues.CountAsync(),
                TotalMeetings = await _context.Meetings.CountAsync(),
                CancelledMeetings = await _context.Meetings
                    .Where(m => m.IsCancelled == true)
                    .CountAsync()
            };

            // 1. Meetings by Department (Top 5)
            var deptStats = await _context.Meetings
                .Include(m => m.Department)
                .GroupBy(m => m.Department!.DepartmentName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            model.DepartmentNames = deptStats.Select(x => x.Name).ToList();
            model.DepartmentCounts = deptStats.Select(x => x.Count).ToList();

            // 2. Meetings by Type (Top 5)
            var typeStats = await _context.Meetings
                .Include(m => m.MeetingType)
                .GroupBy(m => m.MeetingType!.MeetingTypeName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            model.MeetingTypeNames = typeStats.Select(x => x.Name).ToList();
            model.MeetingTypeCounts = typeStats.Select(x => x.Count).ToList();

            // 3. Monthly Trends (Last 6 Months)

            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyStats = await _context.Meetings
               .Where(m => m.MeetingDate >= sixMonthsAgo)
               .OrderBy(m => m.MeetingDate)
               .ToListAsync();

            var groupedMonths = monthlyStats
                .GroupBy(m => m.MeetingDate.HasValue ? m.MeetingDate.Value.ToString("MMM yyyy") : "Unknown")
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            model.MonthLabels = groupedMonths.Select(x => x.Month).ToList();
            model.MonthlyCounts = groupedMonths.Select(x => x.Count).ToList();

            // 4. Venue Utilization (Top 5)
            var venueStats = await _context.Meetings
                .Include(m => m.MeetingVenue)
                .GroupBy(m => m.MeetingVenue!.MeetingVenueName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            model.VenueNames = venueStats.Select(x => x.Name).ToList();
            model.VenueCounts = venueStats.Select(x => x.Count).ToList();

            // 5. Top Staff Contributors (Top 5)
            try
            {
                var staffStats = await _context.Set<MeetingMemberModel>()
                   .Include(mm => mm.Staff)
                   .GroupBy(mm => mm.Staff!.StaffName)
                   .Select(g => new { Name = g.Key, Count = g.Count() })
                   .OrderByDescending(x => x.Count)
                   .Take(5)
                   .ToListAsync();

                model.StaffNames = staffStats.Select(x => x.Name).ToList();
                model.StaffCounts = staffStats.Select(x => x.Count).ToList();
            }
            catch
            {
                model.StaffNames = new List<string>();
                model.StaffCounts = new List<int>();
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
