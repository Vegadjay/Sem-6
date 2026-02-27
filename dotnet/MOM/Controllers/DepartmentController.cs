using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Data;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Save(DepartmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("DepartmentAddEdit", model);
            }

            if (model.DepartmentID == 0)
            {
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _context.Departments.Add(model);
            }
            else
            {
                var existing = _context.Departments.FirstOrDefault(d => d.DepartmentID == model.DepartmentID);
                if (existing != null)
                {
                    existing.DepartmentName = model.DepartmentName;
                    existing.Description = model.Description;
                    existing.Modified = DateTime.Now;
                    _context.Departments.Update(existing);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("DepartmentList");
        }

        public IActionResult DepartmentList()
        {
            var departments = _context.Departments.ToList();
            return View(departments);
        }

        public IActionResult DepartmentAddEdit(int? id)
        {
            if (id.HasValue)
            {
                var department = _context.Departments.FirstOrDefault(d => d.DepartmentID == id.Value);
                if (department != null)
                {
                    return View(department);
                }
            }
            return View(new DepartmentModel());
        }

        public IActionResult Delete(int id)
        {
            var item = _context.Departments.FirstOrDefault(x => x.DepartmentID == id);
            if (item != null)
            {
                _context.Departments.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("DepartmentList");
        }
    }
}
