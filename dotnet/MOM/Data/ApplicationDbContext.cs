using Microsoft.EntityFrameworkCore;
using MOM.Models;

namespace MOM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<MeetingVenueModel> MeetingVenues { get; set; }
        public DbSet<MeetingTypeModel> MeetingTypes { get; set; }
        public DbSet<MeetingStaffModel> MeetingStaff { get; set; }
        public DbSet<MeetingsModel> Meetings { get; set; }
        public DbSet<MeetingMemberModel> MeetingMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships if needed, though EF Core conventions usually handle it.
            
            // Department - Staff (One to Many)
            modelBuilder.Entity<MeetingStaffModel>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Staff)
                .HasForeignKey(s => s.DepartmentID);

            // Meeting - Venue (One to Many)
            modelBuilder.Entity<MeetingsModel>()
                .HasOne(m => m.Venue)
                .WithMany(v => v.Meetings)
                .HasForeignKey(m => m.MeetingVenueId);

            // Meeting - Type (One to Many)
            modelBuilder.Entity<MeetingsModel>()
                .HasOne(m => m.MeetingType)
                .WithMany(t => t.Meetings)
                .HasForeignKey(m => m.MeetingTypeId);

            // Meeting - Department (One to Many, Optional)
            modelBuilder.Entity<MeetingsModel>()
                .HasOne(m => m.Department)
                .WithMany(d => d.Meetings)
                .HasForeignKey(m => m.DepartmentId);

            // MeetingMember - Meeting (One to Many)
            modelBuilder.Entity<MeetingMemberModel>()
                .HasOne(mm => mm.Meeting)
                .WithMany(m => m.MeetingMembers)
                .HasForeignKey(mm => mm.MeetingID);

            // MeetingMember - Staff (One to Many)
            modelBuilder.Entity<MeetingMemberModel>()
                .HasOne(mm => mm.Staff)
                .WithMany(s => s.MeetingMembers)
                .HasForeignKey(mm => mm.StaffID);
        }
    }
}
