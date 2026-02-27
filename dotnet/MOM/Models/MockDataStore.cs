using MOM.Models;

namespace MOM.Data
{
    public static class MockDataStore
    {
        public static List<DepartmentModel> Departments { get; } = new List<DepartmentModel>
        {
            new DepartmentModel { DepartmentID = 1, DepartmentName = "Computer Science", Created = DateTime.Now.AddDays(-10), Modified = DateTime.Now.AddDays(-5) },
            new DepartmentModel { DepartmentID = 2, DepartmentName = "Information Technology", Created = DateTime.Now.AddDays(-8), Modified = DateTime.Now.AddDays(-2) },
            new DepartmentModel { DepartmentID = 3, DepartmentName = "Mechanical Engineering", Created = DateTime.Now.AddDays(-15), Modified = DateTime.Now.AddDays(-1) }
        };

        public static List<MeetingsModel> Meetings { get; } = new List<MeetingsModel>
        {
            new MeetingsModel { MeetingId = 1, MeetingTypeId = 1, MeetingVenueId = 1, MeetingDate = DateTime.Now.AddDays(1), DepartmentId = 1, Created = DateTime.Now, Modified = DateTime.Now, MeetingDescription = "Quarterly board meeting to discuss Q1 results." },
            new MeetingsModel { MeetingId = 2, MeetingTypeId = 2, MeetingVenueId = 2, MeetingDate = DateTime.Now.AddDays(2), DepartmentId = 2, Created = DateTime.Now, Modified = DateTime.Now, MeetingDescription = "Project sync up with the development team." }
        };

        public static List<MeetingTypeModel> MeetingTypes { get; } = new List<MeetingTypeModel>
        {
            new MeetingTypeModel { MeetingTypeId = 1, MeetingTypeName = "Board Meeting", Remarks = "Quarterly board meetings", Created = DateTime.Now.AddDays(-30), Modified = DateTime.Now.AddDays(-10) },
            new MeetingTypeModel { MeetingTypeId = 2, MeetingTypeName = "Committee", Remarks = "Monthly committee sync", Created = DateTime.Now.AddDays(-60), Modified = DateTime.Now.AddDays(-20) }
        };

         public static List<MeetingVenueModel> Venues { get; } = new List<MeetingVenueModel>
        {
            new MeetingVenueModel { MeetingVenueID = 1, MeetingVenueName = "Conference Hall", Address = "Building A, 1st Floor", Created = DateTime.Now.AddDays(-20), Modified = DateTime.Now.AddDays(-5) },
            new MeetingVenueModel { MeetingVenueID = 2, MeetingVenueName = "Meeting Room 1", Address = "Building B, 2nd Floor", Created = DateTime.Now.AddDays(-15), Modified = DateTime.Now.AddDays(-2) }
        };

        public static List<MeetingStaffModel> Staff { get; } = new List<MeetingStaffModel>
        {
            new MeetingStaffModel { StaffID = 1, Name = "John Doe", DepartmentID = 1, Mobile = "1234567890", Email = "john@example.com", Created = DateTime.Now.AddDays(-100), Modified = DateTime.Now },
            new MeetingStaffModel { StaffID = 2, Name = "Jane Smith", DepartmentID = 2, Mobile = "0987654321", Email = "jane@example.com", Created = DateTime.Now.AddDays(-200), Modified = DateTime.Now }
        };

        public static List<MeetingMemberModel> Members { get; } = new List<MeetingMemberModel>
        {
            new MeetingMemberModel { MeetingMemberID = 1, MeetingID = 1, StaffID = 1, IsPresent = true, Remarks = "On time", Created = DateTime.Now.AddDays(-5), Modified = DateTime.Now },
            new MeetingMemberModel { MeetingMemberID = 2, MeetingID = 1, StaffID = 2, IsPresent = false, Remarks = "Sick Leave", Created = DateTime.Now.AddDays(-5), Modified = DateTime.Now }
        };
    }
}
