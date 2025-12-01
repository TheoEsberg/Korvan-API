using Korvan_API.Entities;

namespace Korvan_API.Models.DTOs
{
    public class UpdateShiftDTO
    {
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Notes { get; set; } = string.Empty;

        public Guid? EmployeeId { get; set; }
        public ShiftStatus Status { get; set; }

    }
}
