namespace Korvan_API.Models.DTOs
{
	public class TemplateDetailDTO
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public List<TemplateSlotDTO> Slots { get; set; } = new();
	}
}
