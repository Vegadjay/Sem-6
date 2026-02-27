using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Data;

namespace MOM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalMeetings = _context.Meetings.Count();
            ViewBag.TotalStaff = _context.MeetingStaff.Count();
            ViewBag.TotalDepartments = _context.Departments.Count();
            ViewBag.UpcomingMeetings = _context.Meetings.Count(m => m.MeetingDate > DateTime.Now);
            return View();
        }
    }
}
