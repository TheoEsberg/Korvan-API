namespace Korvan_API.Models.DTOs
{
	public class PublishDayPlanResponseDTO
	{
		public int CreatedShifts { get; set; }
		public int UnassignedSlots { get; set; }
		public List<string> Warnings { get; set; } = new();
	}
}
