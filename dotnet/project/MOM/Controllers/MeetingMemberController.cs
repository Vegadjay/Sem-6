using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class MeetingMemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingMemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.MeetingMembers
                .Include(m => m.Meeting)
                .Include(m => m.Staff)
                .OrderByDescending(m => m.Meeting!.MeetingDate)
                .ToListAsync();

            return View("MeetingMemberList", list);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View("MeetingMemberAddEdit", new MeetingMemberModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingMemberAddEdit", model);
            }

            var parameters = new[]
            {
                new SqlParameter("@MeetingID", (object?)model.MeetingID ?? DBNull.Value),
                new SqlParameter("@StaffID", (object?)model.StaffID ?? DBNull.Value),
                new SqlParameter("@IsPresent", model.IsPresent),
                new SqlParameter("@Remarks", (object?)model.Remarks ?? DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_MeetingMember_Insert @MeetingID, @StaffID, @IsPresent, @Remarks",
                parameters
            );

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var model = await _context.MeetingMembers.FindAsync(id);
            if (model == null) return NotFound();

            await LoadDropdowns();
            return View("MeetingMemberAddEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingMemberAddEdit", model);
            }

            var parameters = new[]
            {
                new SqlParameter("@MeetingMemberID", model.MeetingMemberID),
                new SqlParameter("@MeetingID", (object?)model.MeetingID ?? DBNull.Value),
                new SqlParameter("@StaffID", (object?)model.StaffID ?? DBNull.Value),
                new SqlParameter("@IsPresent", model.IsPresent),
                new SqlParameter("@Remarks", (object?)model.Remarks ?? DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_MeetingMember_UpdateByPK @MeetingMemberID, @MeetingID, @StaffID, @IsPresent, @Remarks",
                parameters
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_MeetingMember_DeleteByPK {0}", id);
                TempData["Success"] = "Attendance record deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this record because it is referenced by other data.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the record.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var meetings = await _context.Meetings.OrderByDescending(m => m.MeetingDate).ToListAsync();
            ViewBag.MeetingList = meetings.Select(m => new { 
                m.MeetingID, 
                DisplayName = (m.MeetingDate?.ToString("dd MMM yyyy") ?? "N/A") + " - " + m.MeetingDescription 
            }).ToList();

            ViewBag.StaffList = await _context.Staff.OrderBy(s => s.StaffName).ToListAsync();
        }
    }
}
