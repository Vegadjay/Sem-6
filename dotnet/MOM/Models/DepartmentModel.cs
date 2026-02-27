using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MOM.Models
{
    public class DepartmentModel
    {
        [Key]
        public int DepartmentID { get; set; }
        
        [DisplayName("Department Name")]
        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters.")]
        public string? DepartmentName { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Department description is required.")]    
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [DisplayName("Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DisplayName("Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<MeetingStaffModel>? Staff { get; set; }
        public virtual ICollection<MeetingsModel>? Meetings { get; set; }
    }
}
