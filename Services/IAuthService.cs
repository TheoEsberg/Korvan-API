using Korvan_API.Entities;
using Korvan_API.Models.DTOs;

namespace Korvan_API.Services
{
	public interface IAuthService
	{
		Task<User?> RegisterAsync(UserDTO request);
		Task<TokenResponseDTO?> LoginAsync(UserDTO request);
		Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request);
	}
}
