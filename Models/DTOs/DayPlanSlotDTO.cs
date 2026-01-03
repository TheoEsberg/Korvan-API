namespace Korvan_API.Models.DTOs
{
	public class DayPlanSlotDTO
	{
		public Guid SlotId { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }

		public Guid? EmployeeId { get; set; }
		public string? EmployeeName { get; set; }
		public string? EmployeeColorHex { get; set; }
		public bool EmployeeHasAvatar { get; set; }
	}
}
