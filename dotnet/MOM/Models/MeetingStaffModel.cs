using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MOM.Models
{
    public class MeetingStaffModel
    {
        [Key]
        public int StaffID { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [DisplayName("Staff Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [DisplayName("Department")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid Mobile Number")]
        [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters.")]
        [DisplayName("Mobile Number")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [DisplayName("Email Address")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        [DisplayName("Remarks")]
        public string? Remarks { get; set; }

        [DisplayName("Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DisplayName("Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual DepartmentModel? Department { get; set; }
        public virtual ICollection<MeetingMemberModel>? MeetingMembers { get; set; }
    }
}
