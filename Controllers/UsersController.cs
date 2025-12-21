using Korvan_API.Data;
using Korvan_API.Extensions;
using Korvan_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Korvan_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly AppDbContext _context;

		public UsersController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserListDTO>>> GetUsers()
		{
			return await _context.Users
				.Where(u => u.IsActive)
				.Select(u => new UserListDTO
				{
					Id = u.Id,
					DisplayName = u.DisplayName
				})
				.ToListAsync();
		}

		[HttpGet("me")]
		[Authorize]
		public async Task<ActionResult<UserProfileDTO>> GetMe()
		{
			var userId = User.GetUserId();
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return NotFound();

			var dto = new UserProfileDTO
			{
				Id = user.Id,
				DisplayName = user.DisplayName,
				Email = user.Email,
				ProfileColorHex = user.ProfileColorHex,
				AvatarUrl = user.AvatarUrl,
				Role = user.Role
			};

			return Ok(dto);
		}

		[HttpPut("me/preferences")]
		[Authorize]
		public async Task<IActionResult> UpdatePreferences([FromBody] UpdateProfilePreferencesDTO dto)
		{
			var userId = User.GetUserId();
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return NotFound();

			user.ProfileColorHex = dto.ProfileColorHex;
			user.AvatarUrl = dto.AvatarUrl;

			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
