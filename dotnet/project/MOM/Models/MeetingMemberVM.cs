
namespace MOM.Models;

public class AttendanceVM
{
    public DateTime MeetingDate { get; set; }

    public string MeetingTypeName { get; set; } = string.Empty;

    public string MeetingVenueName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string StaffName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public bool IsPresent { get; set; }

    public string? Remarks { get; set; }
}
