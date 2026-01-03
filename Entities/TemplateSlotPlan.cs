namespace Korvan_API.Entities
{
	public class TemplateSlotPlan
	{
		public Guid Id { get; set; }

		public Guid DayPlanId { get; set; }
		public TemplateDayPlan DayPlan { get; set; } = null!;

		public Guid SlotId { get; set; }
		public TemplateSlot Slot { get; set; } = null!;

		public Guid? EmployeeId { get; set; }
		public User? Employee { get; set; }
	}
}
