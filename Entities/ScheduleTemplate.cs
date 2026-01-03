namespace Korvan_API.Entities
{
	public class ScheduleTemplate
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;

		public Guid CreatedById { get; set; }
		public User? CreatedBy { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<TemplateSlot> Slots { get; set; } = new List<TemplateSlot>();
	}
}
