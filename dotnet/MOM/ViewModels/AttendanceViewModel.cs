using System.ComponentModel.DataAnnotations;

namespace MOM.ViewModels
{
    public class AttendanceViewModel
    {
        public int MeetingId { get; set; }
        public string? MeetingTitle { get; set; }
        public DateTime MeetingDate { get; set; }
        public List<AttendanceItemViewModel> Attendees { get; set; } = new List<AttendanceItemViewModel>();
    }

    public class AttendanceItemViewModel
    {
        public int StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsPresent { get; set; }
        public string? Remarks { get; set; }
    }
}
