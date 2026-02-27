using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Data;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Save(MeetingTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MeetingTypeAddEdit", model);
            }

            if (model.MeetingTypeId == 0)
            {
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _context.MeetingTypes.Add(model);
            }
            else
            {
                var existing = _context.MeetingTypes.FirstOrDefault(m => m.MeetingTypeId == model.MeetingTypeId);
                if (existing != null)
                {
                    existing.MeetingTypeName = model.MeetingTypeName;
                    existing.Remarks = model.Remarks;
                    existing.Modified = DateTime.Now;
                    _context.MeetingTypes.Update(existing);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("MeetingTypeList");
        }

        public IActionResult MeetingTypeList()
        {
            var types = _context.MeetingTypes.ToList();
            return View(types);
        }

        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (id.HasValue)
            {
                var type = _context.MeetingTypes.FirstOrDefault(m => m.MeetingTypeId == id.Value);
                if (type != null)
                {
                    return View(type);
                }
            }
            return View(new MeetingTypeModel());
        }

        public IActionResult Delete(int id)
        {
            var item = _context.MeetingTypes.FirstOrDefault(x => x.MeetingTypeId == id);
            if (item != null)
            {
                _context.MeetingTypes.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("MeetingTypeList");
        }
    }
}
