namespace Korvan_API.Models.DTOs
{
    public class CreateShiftDTO
    {
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Optional: assign directly to an employee upon creation
        public Guid? EmployeeId { get; set; }
    }
}
