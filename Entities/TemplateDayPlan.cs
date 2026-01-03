namespace Korvan_API.Entities
{
	public class TemplateDayPlan
	{
		public Guid Id { get; set; }
		
		public DateOnly ShiftDate { get; set; }

		public Guid TemplateId { get; set; }
		public ScheduleTemplate Template { get; set; } = null!;

		public bool IsPublished { get; set; } = false;

		public Guid CreatedById { get; set; }
		public User? CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<TemplateSlotPlan> SlotPlans { get; set; } = new List<TemplateSlotPlan>();
	}
}
