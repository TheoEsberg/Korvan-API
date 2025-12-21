using Korvan_API.Entities;

namespace Korvan_API.Models.DTOs
{
    public class WorkShiftDTO
    {
        public Guid Id { get; set; }
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Notes { get; set; } = string.Empty;
        public ShiftStatus Status { get; set; }

        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        public string? EmployeeColorHex { get; set; }
        public string? EmployeeAvatarUrl { get; set; }

        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
