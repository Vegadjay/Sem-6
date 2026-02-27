using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MOM.Models
{
    public class MeetingVenueModel
    {
        [Key]
        public int MeetingVenueID { get; set; }
        [DisplayName("Venue Name")]
        [Required(ErrorMessage = "Venue Name is required.")]
        [StringLength(100, ErrorMessage = "Venue Name cannot exceed 100 characters.")]
        public string? MeetingVenueName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [DisplayName("Address")]
        public string? Address { get; set; }

        [DisplayName("Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [DisplayName("Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DisplayName("Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<MeetingsModel>? Meetings { get; set; }
    }
}
