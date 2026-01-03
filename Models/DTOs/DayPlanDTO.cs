namespace Korvan_API.Models.DTOs
{
	public class DayPlanDTO
	{
		public Guid Id { get; set; }
		public DateOnly ShiftDate { get; set; }
		public Guid TemplateId { get; set; }
		public string TemplateName { get; set; } = string.Empty;
		public bool IsPublished { get; set; }

		public List<DayPlanSlotDTO> Slots { get; set; } = new();
	}
}
