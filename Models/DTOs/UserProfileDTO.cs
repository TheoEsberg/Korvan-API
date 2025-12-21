namespace Korvan_API.Models.DTOs
{
	public class UserProfileDTO
	{
		public Guid Id { get; set; }

		public string Username { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;

		public string Role { get; set; } = string.Empty;

		// New profile preferences
		public string? ProfileColorHex { get; set; }
		public string? AvatarUrl { get; set; }
	}
}
