namespace Korvan_API.Models.DTOs
{
	public class TemplateSlotDTO
	{
		public Guid Id { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public int SortOrder { get; set; }
	}
}
