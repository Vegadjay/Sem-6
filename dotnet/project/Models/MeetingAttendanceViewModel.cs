namespace MOM.Models
{
    public class MeetingAttendanceViewModel
    {
        public int MemberID { get; set; }
        public int MeetingID { get; set; }
        public int StaffID { get; set; }
        public string? Attendance { get; set; }
        public string? Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
