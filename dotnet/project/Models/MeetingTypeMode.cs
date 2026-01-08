namespace MOM.Models
{
    public class MeetingTypeModel
    {
        public int MeetingTypeId { get; set; }
        public string? MeetingTypeName { get; set; }
        public string? Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
