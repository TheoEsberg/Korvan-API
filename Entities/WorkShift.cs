namespace Korvan_API.Entities
{
    public enum ShiftStatus
    {
        Draft,          // Owner created but not yet published
        Published,      // Visible to employees
        Offered,        // Put up for grabs / swap
        Taken,          // Accepted by another employee
        Cancelled       // Cancelled by owner or admin
    }
    
    public class WorkShift
    {
        public Guid Id { get; set; }

        // --- Shift Time Details ---
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        // --- Shift Metadata ---
        public string Notes { get; set; } = string.Empty;
        public ShiftStatus Status { get; set; } = ShiftStatus.Draft;

        // --- Relationships ---
        public Guid? EmployeeId { get; set; }
        public User? Employee { get; set; }

        // --- Audit Fields ---
        public Guid CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
