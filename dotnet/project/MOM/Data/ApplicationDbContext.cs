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

        public DbSet<MeetingsModel> Meetings { get; set; }
        public DbSet<MeetingMemberModel> MeetingMembers { get; set; }
        public DbSet<StaffModel> Staff { get; set; }
        public DbSet<MeetingTypeModel> MeetingTypes { get; set; }
        public DbSet<MeetingVenueModel> MeetingVenues { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }

        public virtual DbSet<MeetingListVM> MeetingListVM { get; set; } = null!;
        public virtual DbSet<StaffListVM> StaffListVM { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeetingListVM>().HasNoKey().ToView(null);
            modelBuilder.Entity<StaffListVM>(entity =>
            {
                entity.HasNoKey().ToView(null);
                entity.Ignore(s => s.TotalMeetings);
                entity.Ignore(s => s.AttendanceRate);
            });
        }
    }
}
