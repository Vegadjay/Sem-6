using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models;

public class StaffListVM
{
    public int StaffID { get; set; }

    public string? StaffName { get; set; }
    public string? DepartmentName { get; set; }

    public string? EmailAddress { get; set; }
    public string? MobileNo { get; set; }

    [NotMapped]
    public int? TotalMeetings { get; set; }
    [NotMapped]
    public double? AttendanceRate { get; set; }
}
