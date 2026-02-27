using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Models;
using MOM.Data;

namespace MOM.Controllers
{
    public class MeetingMemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingMemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Save(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Meetings = _context.Meetings.ToList();
                ViewBag.Staff = _context.MeetingStaff.ToList();
                return View("MeetingMemberAddEdit", model);
            }

            if (model.MeetingMemberID == 0)
            {
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _context.MeetingMembers.Add(model);
            }
            else
            {
                var existing = _context.MeetingMembers.FirstOrDefault(m => m.MeetingMemberID == model.MeetingMemberID);
                if (existing != null)
                {
                    existing.MeetingID = model.MeetingID;
                    existing.StaffID = model.StaffID;
                    existing.IsPresent = model.IsPresent;
                    existing.Remarks = model.Remarks;
                    existing.Modified = DateTime.Now;
                    _context.MeetingMembers.Update(existing);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("MeetingMemberList");
        }

        public IActionResult MeetingMemberList()
        {
            var members = _context.MeetingMembers
                .Include(m => m.Meeting)
                .Include(m => m.Staff)
                .OrderByDescending(m => m.Meeting != null ? m.Meeting.MeetingDate : DateTime.MinValue)
                .ThenBy(m => m.Staff != null ? m.Staff.Name : string.Empty)
                .ToList();
            return View(members);
        }

        public IActionResult MeetingMemberAddEdit(int? id)
        {
            ViewBag.Meetings = _context.Meetings.ToList();
            ViewBag.Staff = _context.MeetingStaff.ToList();

            if (id.HasValue)
            {
                var member = _context.MeetingMembers
                    .Include(m => m.Meeting)
                    .Include(m => m.Staff)
                    .FirstOrDefault(m => m.MeetingMemberID == id.Value);
                if (member != null)
                {
                    return View(member);
                }
            }
            return View(new MeetingMemberModel());
        }

        public IActionResult Delete(int id)
        {
            var item = _context.MeetingMembers.FirstOrDefault(x => x.MeetingMemberID == id);
            if (item != null)
            {
                _context.MeetingMembers.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("MeetingMemberList");
        }
    }
}
