using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Models;
using MOM.Data;
using MOM.ViewModels;

namespace MOM.Controllers
{
    public class MeetingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Save(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MeetingTypes = _context.MeetingTypes.ToList();
                ViewBag.Venues = _context.MeetingVenues.ToList();
                ViewBag.Departments = _context.Departments.ToList();
                return View("MeetingAddEdit", model);
            }

            if (model.MeetingId == 0)
            {
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _context.Meetings.Add(model);
            }
            else
            {
                var existing = _context.Meetings.FirstOrDefault(m => m.MeetingId == model.MeetingId);
                if (existing != null)
                {
                    existing.MeetingTypeId = model.MeetingTypeId;
                    existing.MeetingVenueId = model.MeetingVenueId;
                    existing.DepartmentId = model.DepartmentId;
                    existing.MeetingDate = model.MeetingDate;
                    existing.MeetingDescription = model.MeetingDescription;
                    existing.Modified = DateTime.Now;
                    _context.Meetings.Update(existing);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("MeetingList");
        }

        public IActionResult MeetingList()
        {
            var meetings = _context.Meetings
                .Include(m => m.MeetingType)
                .Include(m => m.Venue)
                .Include(m => m.Department)
                .ToList();
            return View(meetings);
        }

        public IActionResult MeetingAddEdit(int? id)
        {
            ViewBag.MeetingTypes = _context.MeetingTypes.ToList();
            ViewBag.Venues = _context.MeetingVenues.ToList();
            ViewBag.Departments = _context.Departments.ToList();

            if (id.HasValue)
            {
                var meeting = _context.Meetings.FirstOrDefault(m => m.MeetingId == id.Value);
                if (meeting != null)
                {
                    return View(meeting);
                }
            }
            return View(new MeetingsModel { MeetingDate = DateTime.Now });
        }

        public IActionResult Attandance(int? id)
        {
            ViewBag.Meetings = _context.Meetings.OrderByDescending(m => m.MeetingDate).ToList();

            if (!id.HasValue)
            {
                return View(new AttendanceViewModel());
            }

            var meeting = _context.Meetings
                .Include(m => m.MeetingType)
                .Include(m => m.Venue)
                .FirstOrDefault(m => m.MeetingId == id.Value);

            if (meeting == null) return NotFound();

            var staffList = _context.MeetingStaff
                .Include(s => s.Department)
                .ToList();

            var existingAttendance = _context.MeetingMembers
                .Where(m => m.MeetingID == id.Value)
                .ToList();

            var viewModel = new AttendanceViewModel
            {
                MeetingId = meeting.MeetingId,
                MeetingTitle = meeting.MeetingDescription,
                MeetingDate = meeting.MeetingDate,
                Attendees = staffList.Select(s => new AttendanceItemViewModel
                {
                    StaffId = s.StaffID,
                    StaffName = s.Name,
                    DepartmentName = s.Department?.DepartmentName,
                    IsPresent = existingAttendance.Any(a => a.StaffID == s.StaffID && a.IsPresent),
                    Remarks = existingAttendance.FirstOrDefault(a => a.StaffID == s.StaffID)?.Remarks
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SaveAttendance(AttendanceViewModel model)
        {
            var existingMembers = _context.MeetingMembers
                .Where(m => m.MeetingID == model.MeetingId)
                .ToList();

            foreach (var attendee in model.Attendees)
            {
                var member = existingMembers.FirstOrDefault(m => m.StaffID == attendee.StaffId);

                if (member == null)
                {
                    // Add new record
                    _context.MeetingMembers.Add(new MeetingMemberModel
                    {
                        MeetingID = model.MeetingId,
                        StaffID = attendee.StaffId,
                        IsPresent = attendee.IsPresent,
                        Remarks = attendee.Remarks,
                        Created = DateTime.Now,
                        Modified = DateTime.Now
                    });
                }
                else
                {
                    // Update existing record
                    member.IsPresent = attendee.IsPresent;
                    member.Remarks = attendee.Remarks;
                    member.Modified = DateTime.Now;
                    _context.MeetingMembers.Update(member);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Attandance", new { id = model.MeetingId });
        }

        public IActionResult Delete(int id)
        {
            var item = _context.Meetings.FirstOrDefault(x => x.MeetingId == id);
            if (item != null)
            {
                _context.Meetings.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("MeetingList");
        }
    }
}
