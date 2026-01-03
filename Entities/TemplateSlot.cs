namespace Korvan_API.Entities
{
	public class TemplateSlot
	{
		public Guid Id { get; set; }
		

		public Guid TemplateId { get; set; }
		public ScheduleTemplate Template { get; set; } = null!;

		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }

		public int SortOrder { get; set; }
	}
}
