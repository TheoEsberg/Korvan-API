namespace Korvan_API.Models.DTOs
{
	public class CreateTemplateSlotDTO
	{
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public int SortOrder { get; set; }
	}
}
