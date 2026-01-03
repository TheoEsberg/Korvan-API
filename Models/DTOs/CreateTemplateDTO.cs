namespace Korvan_API.Models.DTOs
{
	public class CreateTemplateDTO
	{
		public string Name { get; set; } = string.Empty;
		public List<CreateTemplateSlotDTO> Slots { get; set; } = new();
	}
}
