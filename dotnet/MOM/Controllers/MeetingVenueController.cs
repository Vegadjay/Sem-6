using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Data;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingVenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Save(MeetingVenueModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MeetingVenueAddEdit", model);
            }

            if (model.MeetingVenueID == 0)
            {
                model.Created = DateTime.Now;
                model.Modified = DateTime.Now;
                _context.MeetingVenues.Add(model);
            }
            else
            {
                var existing = _context.MeetingVenues.FirstOrDefault(v => v.MeetingVenueID == model.MeetingVenueID);
                if (existing != null)
                {
                    existing.MeetingVenueName = model.MeetingVenueName;
                    existing.Address = model.Address;
                    existing.Description = model.Description;
                    existing.Modified = DateTime.Now;
                    _context.MeetingVenues.Update(existing);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("MeetingVenueList");
        }

        public IActionResult MeetingVenueList()
        {
            var venues = _context.MeetingVenues.ToList();
            return View(venues);
        }

        public IActionResult MeetingVenueAddEdit(int? id)
        {
            if (id.HasValue)
            {
                var venue = _context.MeetingVenues.FirstOrDefault(v => v.MeetingVenueID == id.Value);
                if (venue != null)
                {
                    return View(venue);
                }
            }
            return View(new MeetingVenueModel());
        }

        public IActionResult Delete(int id)
        {
            var item = _context.MeetingVenues.FirstOrDefault(x => x.MeetingVenueID == id);
            if (item != null)
            {
                _context.MeetingVenues.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("MeetingVenueList");
        }
    }
}
