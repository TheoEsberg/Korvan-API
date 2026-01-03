using Korvan_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korvan_API.Data
{
	public class AppDbContext : DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) { } 

		public DbSet<User> Users => Set<User>();
		public DbSet<WorkShift> WorkShifts => Set<WorkShift>();
		public DbSet<ShiftOffer> ShiftOffers => Set<ShiftOffer>();
		public DbSet<Notification> Notifications => Set<Notification>();
		public DbSet<ScheduleTemplate> ScheduleTemplates => Set<ScheduleTemplate>();
		public DbSet<TemplateSlot> TemplateSlots => Set<TemplateSlot>();
		public DbSet<TemplateDayPlan> TemplateDayPlans => Set<TemplateDayPlan>();
		public DbSet<TemplateSlotPlan> TemplateSlotPlans => Set<TemplateSlotPlan>();


		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ WorkShift (assigned employee)
            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedShifts)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // WorkShift ↔ CreatedBy
            modelBuilder.Entity<WorkShift>()
                .HasOne(s => s.CreatedBy)
                .WithMany()
                .HasForeignKey(s => s.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkShift ↔ UpdatedBy
            modelBuilder.Entity<WorkShift>()
                .HasOne(s => s.UpdatedBy)
                .WithMany()
                .HasForeignKey(s => s.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ShiftOffer ↔ Shift
            modelBuilder.Entity<ShiftOffer>()
                .HasOne(o => o.Shift)
                .WithMany()
                .HasForeignKey(o => o.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShiftOffer ↔ OfferedBy
            modelBuilder.Entity<ShiftOffer>()
                .HasOne(o => o.OfferedBy)
                .WithMany()
                .HasForeignKey(o => o.OfferedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ShiftOffer ↔ TakenBy
            modelBuilder.Entity<ShiftOffer>()
                .HasOne(o => o.TakenBy)
                .WithMany()
                .HasForeignKey(o => o.TakenById)
                .OnDelete(DeleteBehavior.SetNull);

            // Notification ↔ User
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
