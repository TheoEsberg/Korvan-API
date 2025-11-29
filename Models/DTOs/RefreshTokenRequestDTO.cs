namespace Korvan_API.Models.DTOs
{
	public class RefreshTokenRequestDTO
	{
		public Guid UserId { get; set; }
		public required string RefreshToken { get; set; }
	}
}
