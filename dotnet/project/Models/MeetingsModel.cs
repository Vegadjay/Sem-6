namespace MOM.Models
{
    public class MeetingsModel
    {
        public int MeetingId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? MeetingVenue { get; set; }
        public string? MeetingType { get; set; }
        public string? DepartmentID { get; set; }
        public string? MeetingDescription { get; set; }
        public string? DocumentPath { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Boolean IsCancelled {get; set;}
        public DateTime CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
    }
}
