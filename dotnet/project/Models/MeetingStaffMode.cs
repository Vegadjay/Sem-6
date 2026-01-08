namespace MOM.Models
{
    public class MeetingStaffModel
    {
        public int StaffID { get; set; }
        public int DepartmentID { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
