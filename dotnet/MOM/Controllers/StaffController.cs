using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Models;
using MOM.Data;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Save(MeetingStaffModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View("StaffAddEdit", model);
            }

            if (model.StaffID == 0)
            {
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _context.MeetingStaff.Add(model);
            }
            else
            {
                var existing = _context.MeetingStaff.FirstOrDefault(s => s.StaffID == model.StaffID);
                if (existing != null)
                {
                    existing.Name = model.Name;
                    existing.DepartmentID = model.DepartmentID;
                    existing.Mobile = model.Mobile;
                    existing.Email = model.Email;
                    existing.Remarks = model.Remarks;
                    existing.Modified = DateTime.Now;
                    _context.MeetingStaff.Update(existing);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("StaffList");
        }

        public IActionResult StaffList()
        {
            var staff = _context.MeetingStaff
                .Include(s => s.Department)
                .ToList();
            return View(staff);
        }

        public IActionResult StaffAddEdit(int? id)
        {
            ViewBag.Departments = _context.Departments.ToList();

            if (id.HasValue)
            {
                var staff = _context.MeetingStaff.FirstOrDefault(s => s.StaffID == id.Value);
                if (staff != null)
                {
                    return View(staff);
                }
            }
            return View(new MeetingStaffModel());
        }

        public IActionResult Delete(int id)
        {
            var item = _context.MeetingStaff.FirstOrDefault(x => x.StaffID == id);
            if (item != null)
            {
                _context.MeetingStaff.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("StaffList");
        }
    }
}
