using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MOM.Models
{
    public class MeetingMemberModel
    {
        [Key]
        public int MeetingMemberID { get; set; }

        [Required(ErrorMessage = "Meeting is required")]
        [DisplayName("Meeting")]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Staff member is required")]
        [DisplayName("Staff Member")]
        public int StaffID { get; set; }

        [DisplayName("Present")]
        public Boolean IsPresent { get; set; }

        [DisplayName("Remarks")]
        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        [DisplayName("Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DisplayName("Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual MeetingsModel? Meeting { get; set; }
        public virtual MeetingStaffModel? Staff { get; set; }
    }
}
