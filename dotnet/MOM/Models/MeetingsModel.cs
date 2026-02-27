using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MOM.Models
{
    public class MeetingsModel
    {
        [Key]
        public int MeetingId { get; set; }
        [Required(ErrorMessage = "Meeting Date is required")]
        [DisplayName("Meeting Date")]
        public DateTime MeetingDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Meeting Venue is required")]
        [DisplayName("Venue")]
        public int MeetingVenueId { get; set; }

        [Required(ErrorMessage = "Meeting Type is required")]
        [DisplayName("Meeting Type")]
        public int MeetingTypeId { get; set; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; } // Optional

        [Required(ErrorMessage = "Description is required")]
        [DisplayName("Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? MeetingDescription { get; set; }

        [DisplayName("Document Path")]
        [StringLength(255)]
        public string? DocumentPath { get; set; }

        [DisplayName("Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DisplayName("Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        [DisplayName("Cancelled")]
        public Boolean IsCancelled {get; set;}

        [DisplayName("Cancellation Date")]
        public DateTime? CancellationDate { get; set; }

        [DisplayName("Cancellation Reason")]
        [StringLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters.")]
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual MeetingVenueModel? Venue { get; set; }
        public virtual MeetingTypeModel? MeetingType { get; set; }
        public virtual DepartmentModel? Department { get; set; }
        public virtual ICollection<MeetingMemberModel>? MeetingMembers { get; set; }
    }
}
