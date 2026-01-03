using Korvan_API.Data;
using Korvan_API.Entities;
using Korvan_API.Extensions;
using Korvan_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Korvan_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class TemplatesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public TemplatesController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/templates
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TemplateListDTO>>> GetTemplates()
		{
			var list = await _context.ScheduleTemplates
				.AsNoTracking()
				.OrderBy(t => t.Name)
				.Select(t => new TemplateListDTO
				{
					Id = t.Id,
					Name = t.Name
				})
				.ToListAsync();

			return Ok(list);
		}

		// GET: api/templates/{id}
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<TemplateDetailDTO>> GetTemplateById(Guid id)
		{
			var template = await _context.ScheduleTemplates
				.AsNoTracking()
				.Include(t => t.Slots)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (template == null) return NotFound();

			var dto = new TemplateDetailDTO
			{
				Id = template.Id,
				Name = template.Name,
				Slots = template.Slots
					.OrderBy(s => s.SortOrder)
					.ThenBy(s => s.StartTime)
					.Select(s => new TemplateSlotDTO
					{
						Id = s.Id,
						StartTime = s.StartTime,
						EndTime = s.EndTime,
						SortOrder = s.SortOrder
					})
					.ToList()
			};

			return Ok(dto);
		}

		// POST: api/templates
		[HttpPost]
		public async Task<ActionResult<TemplateDetailDTO>> CreateTemplate([FromBody] CreateTemplateDTO dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Name))
				return BadRequest("Template name is required.");

			if (dto.Slots == null || dto.Slots.Count == 0)
				return BadRequest("Template must contain at least one slot.");

			// Basic validation: no invalid times
			foreach (var s in dto.Slots)
			{
				if (s.EndTime <= s.StartTime)
					return BadRequest("Slot end time must be after start time.");
			}

			var creatorId = User.GetUserId();

			var template = new ScheduleTemplate
			{
				Name = dto.Name.Trim(),
				CreatedById = creatorId,
				CreatedAt = DateTime.UtcNow,
				Slots = dto.Slots
					.OrderBy(s => s.SortOrder)
					.ThenBy(s => s.StartTime)
					.Select(s => new TemplateSlot
					{
						StartTime = s.StartTime,
						EndTime = s.EndTime,
						SortOrder = s.SortOrder
					})
					.ToList()
			};

			_context.ScheduleTemplates.Add(template);
			await _context.SaveChangesAsync();

			// Return detail dto
			var result = new TemplateDetailDTO
			{
				Id = template.Id,
				Name = template.Name,
				Slots = template.Slots
					.OrderBy(s => s.SortOrder)
					.ThenBy(s => s.StartTime)
					.Select(s => new TemplateSlotDTO
					{
						Id = s.Id,
						StartTime = s.StartTime,
						EndTime = s.EndTime,
						SortOrder = s.SortOrder
					})
					.ToList()
			};

			return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, result);
		}
	}
}
