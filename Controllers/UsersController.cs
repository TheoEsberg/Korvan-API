using Korvan_API.Data;
using Korvan_API.Extensions;
using Korvan_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;

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
				Role = user.Role,
				HasAvatar = user.AvatarImage != null && user.AvatarImage.Length > 0
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

			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPost("me/avatar")]
		[Authorize]
		[Consumes("multipart/form-data")]
		[RequestSizeLimit(5 * 1024 * 1024)] // 5 MB limit
		public async Task<IActionResult> UploadMyAvatar([FromForm] IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

			var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".heic", ".heif" };
			if (!allowedExts.Contains(ext))
			{
				return BadRequest($"Unsupported file type. Filename: {file.FileName}, ContentType: {file.ContentType}");
			}

			var userId = User.GetUserId();
			var user = await _context.Users.FindAsync(userId);
			if(user == null) { return NotFound(); }

			await using var ms = new MemoryStream();
			await file.CopyToAsync(ms);

			user.AvatarImage = ms.ToArray();
			user.AvatarContentType = file.ContentType;

		    await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("me/avatar")]
		[Authorize]
		public async Task<IActionResult> DeleteMyAvatar()
		{
			var userId = User.GetUserId();
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return NotFound();

			user.AvatarImage = null;
			user.AvatarContentType = null;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpGet("me/avatar")]
		[AllowAnonymous]
		[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)] // Cache for 1 hour
		public async Task<IActionResult> GetMyAvatar()
		{
			var userId = User.GetUserId();
			var user = await _context.Users
			   .AsNoTracking()
			   .FirstOrDefaultAsync(u => u.Id == userId);
			if (user?.AvatarImage == null || user.AvatarContentType == null)
			{
				return NotFound();
			}

			return File(user.AvatarImage, user.AvatarContentType);
		}

		[HttpGet("{id:guid}/avatar")]
		[AllowAnonymous]
		[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
		public async Task<IActionResult> GetUserAvatar(Guid id)
		{
			var user = await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Id == id);

			if (user?.AvatarImage == null || user.AvatarContentType == null)
				return NotFound();

			return File(user.AvatarImage, user.AvatarContentType);
		}
	}
}
