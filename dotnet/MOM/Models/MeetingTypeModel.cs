using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MOM.Models
{
    public class MeetingTypeModel
    {
        [Key]
        public int MeetingTypeId { get; set; }
        [DisplayName("Meeting Type Name")]
        [Required(ErrorMessage = "Meeting Type Name is required.")]
        [StringLength(100, ErrorMessage = "Meeting Type Name cannot exceed 100 characters.")]
        public string? MeetingTypeName { get; set; }

        [DisplayName("Remarks")]
        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        [DisplayName("Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DisplayName("Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<MeetingsModel>? Meetings { get; set; }
    }
}
