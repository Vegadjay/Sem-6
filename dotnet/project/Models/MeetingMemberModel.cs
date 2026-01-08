namespace MOM.Models
{
    public class MeetingMemberModel
    {
        public int MeetingMemberID { get; set; }
        public int MeetingID { get; set; }
        public int StaffID { get; set; }
        public Boolean IsPresent { get; set; }
        public string? Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; } 
    }
}
