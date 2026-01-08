namespace MOM.Models
{
    public class DepartmentModel
    {
        public int MeetingVenueID { get; set; }
        
        public string? MeetingVenueName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
