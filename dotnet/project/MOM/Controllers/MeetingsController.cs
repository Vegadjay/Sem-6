using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            int? MeetingTypeID = null,
            int? DepartmentID = null,
            int? MeetingVenueID = null,
            DateTime? StartDate = null,
            DateTime? EndDate = null,
            string? SearchText = null,
            int? page = null)
        {
            await LoadDropdowns();

            // Set current filter values for the view
            ViewBag.CurrentMeetingTypeID = MeetingTypeID;
            ViewBag.CurrentDepartmentID = DepartmentID;
            ViewBag.CurrentMeetingVenueID = MeetingVenueID;
            ViewBag.CurrentStartDate = StartDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = EndDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentSearchText = SearchText;

            // Execute search stored procedure via EF Core
            var parameters = new[]
            {
                new SqlParameter("@MeetingTypeID", (object?)MeetingTypeID ?? DBNull.Value),
                new SqlParameter("@DepartmentID", (object?)DepartmentID ?? DBNull.Value),
                new SqlParameter("@MeetingVenueID", (object?)MeetingVenueID ?? DBNull.Value),
                new SqlParameter("@StartDate", (object?)StartDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object?)EndDate ?? DBNull.Value),
                new SqlParameter("@SearchText", (object?)SearchText ?? DBNull.Value)
            };

            var meetings = await _context.Set<MeetingListVM>()
                .FromSqlRaw("EXEC PR_MOM_Search_Meetings @MeetingTypeID, @DepartmentID, @MeetingVenueID, @StartDate, @EndDate, @SearchText", parameters)
                .ToListAsync();

            // Pagination logic
            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = meetings.Count;
            var pagedList = meetings.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View("MeetingList", pagedList);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View("MeetingAddEdit", new MeetingsModel 
            { 
                MeetingDate = DateTime.Now.AddMinutes(1).AddSeconds(-DateTime.Now.Second) // Set to current time rounded to next minute
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingAddEdit", model);
            }

            var parameters = new[]
            {
                new SqlParameter("@MeetingDate", (object?)model.MeetingDate ?? DBNull.Value),
                new SqlParameter("@MeetingVenueID", (object?)model.MeetingVenueID ?? DBNull.Value),
                new SqlParameter("@MeetingTypeID", (object?)model.MeetingTypeID ?? DBNull.Value),
                new SqlParameter("@DepartmentID", (object?)model.DepartmentID ?? DBNull.Value),
                new SqlParameter("@MeetingDescription", (object?)model.MeetingDescription ?? DBNull.Value),
                new SqlParameter("@DocumentPath", (object?)model.DocumentPath ?? DBNull.Value),
                new SqlParameter("@IsCancelled", false),
                new SqlParameter("@CancellationDateTime", DBNull.Value),
                new SqlParameter("@CancellationReason", DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_Meetings_Insert @MeetingDate, @MeetingVenueID, @MeetingTypeID, @DepartmentID, @MeetingDescription, @DocumentPath, @IsCancelled, @CancellationDateTime, @CancellationReason",
                parameters
            );
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.MeetingType)
                .Include(m => m.Department)
                .Include(m => m.MeetingVenue)
                .Include(m => m.MeetingMembers!)
                    .ThenInclude(mm => mm.Staff)
                        .ThenInclude(s => s!.Department)
                .FirstOrDefaultAsync(m => m.MeetingID == id);

            if (meeting == null) return NotFound();

            // Fetch staff members not already in this meeting for the "Add Member" dropdown
            var currentMemberIds = meeting.MeetingMembers?.Select(mm => mm.StaffID).ToList() ?? new List<int?>();
            ViewBag.StaffList = await _context.Staff
                .Include(s => s.Department)
                .Where(s => !currentMemberIds.Contains(s.StaffID))
                .ToListAsync();

            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(int meetingId, int staffId)
        {
            var parameters = new[]
            {
                new SqlParameter("@MeetingID", meetingId),
                new SqlParameter("@StaffID", staffId),
                new SqlParameter("@IsPresent", false),
                new SqlParameter("@Remarks", "")
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_MeetingMember_Insert @MeetingID, @StaffID, @IsPresent, @Remarks",
                parameters
            );
            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int meetingMemberId, int meetingId)
        {
            var member = await _context.MeetingMembers.FindAsync(meetingMemberId);
            if (member != null)
            {
                _context.MeetingMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        public async Task<IActionResult> Update(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null) return NotFound();

            await LoadDropdowns();
            return View("MeetingAddEdit", meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View("MeetingAddEdit", model);
            }

            var parameters = new[]
            {
                new SqlParameter("@MeetingID", model.MeetingID),
                new SqlParameter("@MeetingDate", (object?)model.MeetingDate ?? DBNull.Value),
                new SqlParameter("@MeetingVenueID", (object?)model.MeetingVenueID ?? DBNull.Value),
                new SqlParameter("@MeetingTypeID", (object?)model.MeetingTypeID ?? DBNull.Value),
                new SqlParameter("@DepartmentID", (object?)model.DepartmentID ?? DBNull.Value),
                new SqlParameter("@MeetingDescription", (object?)model.MeetingDescription ?? DBNull.Value),
                new SqlParameter("@DocumentPath", (object?)model.DocumentPath ?? DBNull.Value),
                new SqlParameter("@IsCancelled", (object?)model.IsCancelled ?? DBNull.Value),
                new SqlParameter("@CancellationDateTime", (object?)model.CancellationDateTime ?? DBNull.Value),
                new SqlParameter("@CancellationReason", (object?)model.CancellationReason ?? DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_Meetings_UpdateByPK @MeetingID, @MeetingDate, @MeetingVenueID, @MeetingTypeID, @DepartmentID, @MeetingDescription, @DocumentPath, @IsCancelled, @CancellationDateTime, @CancellationReason",
                parameters
            );
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttendance(List<MeetingMemberModel> members, int meetingId)
        {
            if (members != null && members.Any())
            {
                foreach (var member in members)
                {
                    var existingMember = await _context.MeetingMembers.FindAsync(member.MeetingMemberID);
                    if (existingMember != null)
                    {
                        existingMember.IsPresent = member.IsPresent;
                        existingMember.Modified = DateTime.Now;
                        _context.Entry(existingMember).Property(x => x.IsPresent).IsModified = true;
                        _context.Entry(existingMember).Property(x => x.Modified).IsModified = true;
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var parameter = new SqlParameter("@MeetingID", id);
                await _context.Database.ExecuteSqlRawAsync("EXEC PR_MOM_Meetings_DeleteByPK @MeetingID", parameter);
                TempData["Success"] = "Meeting deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the meeting.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            ViewBag.MeetingTypeList = await _context.MeetingTypes.FromSqlRaw("EXEC PR_MOM_MeetingType_SelectAll").ToListAsync();
            ViewBag.MeetingVenueList = await _context.MeetingVenues.FromSqlRaw("EXEC PR_MOM_MeetingVenue_SelectAll").ToListAsync();
            ViewBag.DepartmentList = await _context.Departments.FromSqlRaw("EXEC PR_MOM_Department_SelectAll").ToListAsync();
        }
    }
}
