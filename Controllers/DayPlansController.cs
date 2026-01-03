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
	[Route("api/admin/day-plans")]
	[Authorize(Roles = "Admin")]
	public class DayPlansController : ControllerBase
	{
		private readonly AppDbContext _context;

		public DayPlansController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/admin/day-plans?date=2026-01-03&templateId=...
		// Get or create a day plan for a specific date + template
		[HttpGet]
		public async Task<ActionResult<DayPlanDTO>> GetOrCreateDayPlan(
			[FromQuery] DateOnly date,
			[FromQuery] Guid templateId)
		{
			var plan = await _context.TemplateDayPlans
				.Include(p => p.Template)
				.Include(p => p.SlotPlans)
					.ThenInclude(sp => sp.Slot)
				.Include(p => p.SlotPlans)
					.ThenInclude(sp => sp.Employee)
				.FirstOrDefaultAsync(p => p.ShiftDate == date && p.TemplateId == templateId);

			if (plan == null)
			{
				var template = await _context.ScheduleTemplates
					.Include(t => t.Slots)
					.FirstOrDefaultAsync(t => t.Id == templateId);

				if (template == null) return NotFound("Template not found.");

				var creatorId = User.GetUserId();

				plan = new TemplateDayPlan
				{
					ShiftDate = date,
					TemplateId = template.Id,
					CreatedById = creatorId,
					CreatedAt = DateTime.UtcNow,
					IsPublished = false,
					SlotPlans = template.Slots
						.OrderBy(s => s.SortOrder)
						.ThenBy(s => s.StartTime)
						.Select(s => new TemplateSlotPlan
						{
							SlotId = s.Id,
							EmployeeId = null
						})
						.ToList()
				};

				_context.TemplateDayPlans.Add(plan);
				await _context.SaveChangesAsync();

				// Reload with includes
				plan = await _context.TemplateDayPlans
					.Include(p => p.Template)
					.Include(p => p.SlotPlans).ThenInclude(sp => sp.Slot)
					.Include(p => p.SlotPlans).ThenInclude(sp => sp.Employee)
					.FirstAsync(p => p.Id == plan.Id);
			}

			return Ok(ToDayPlanDTO(plan));
		}

		// PUT: api/admin/day-plans/{dayPlanId}/slots/{slotId}
		// Body: { employeeId: "guid" } or { employeeId: null } to clear
		[HttpPut("{dayPlanId:guid}/slots/{slotId:guid}")]
		public async Task<IActionResult> AssignEmployeeToSlot(
			Guid dayPlanId,
			Guid slotId,
			[FromBody] AssignSlotDTO dto)
		{
			var plan = await _context.TemplateDayPlans
				.Include(p => p.SlotPlans)
				.FirstOrDefaultAsync(p => p.Id == dayPlanId);

			if (plan == null) return NotFound("Day plan not found.");

			if (plan.IsPublished)
				return BadRequest("This day is already published and cannot be edited.");

			var slotPlan = plan.SlotPlans.FirstOrDefault(sp => sp.SlotId == slotId);
			if (slotPlan == null) return NotFound("Slot not found in day plan.");

			if (dto.EmployeeId.HasValue)
			{
				var employeeExists = await _context.Users.AnyAsync(u => u.Id == dto.EmployeeId.Value);
				if (!employeeExists) return BadRequest("Employee not found.");
			}

			slotPlan.EmployeeId = dto.EmployeeId; // can be null to clear
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// POST: api/admin/day-plans/{dayPlanId}/publish
		[HttpPost("{dayPlanId:guid}/publish")]
		public async Task<ActionResult<PublishDayPlanResponseDTO>> PublishDayPlan(Guid dayPlanId)
		{
			var dayPlan = await _context.TemplateDayPlans
				.Include(p => p.SlotPlans).ThenInclude(sp => sp.Slot)
				.FirstOrDefaultAsync(p => p.Id == dayPlanId);

			if (dayPlan == null) return NotFound();

			if (dayPlan.IsPublished)
				return BadRequest("This day is already published.");

			var creatorId = User.GetUserId();

			int created = 0;
			int unassigned = 0;

			foreach (var sp in dayPlan.SlotPlans
						 .OrderBy(x => x.Slot.SortOrder)
						 .ThenBy(x => x.Slot.StartTime))
			{
				if (sp.EmployeeId == null)
				{
					unassigned++;
					continue;
				}

				var shift = new WorkShift
				{
					ShiftDate = dayPlan.ShiftDate,
					StartTime = sp.Slot.StartTime,
					EndTime = sp.Slot.EndTime,
					Notes = "",
					Status = ShiftStatus.Published,   // adjust if your property name differs
					EmployeeId = sp.EmployeeId,
					CreatedById = creatorId,
					CreatedAt = DateTime.UtcNow
				};

				_context.WorkShifts.Add(shift);
				created++;
			}

			dayPlan.IsPublished = true;
			await _context.SaveChangesAsync();

			var resp = new PublishDayPlanResponseDTO
			{
				CreatedShifts = created,
				UnassignedSlots = unassigned
			};

			if (unassigned > 0)
				resp.Warnings.Add($"{unassigned} slot(s) were unassigned and were not published.");

			return Ok(resp);
		}

		// ---------- Helpers ----------
		private DayPlanDTO ToDayPlanDTO(TemplateDayPlan plan)
		{
			return new DayPlanDTO
			{
				Id = plan.Id,
				ShiftDate = plan.ShiftDate,
				TemplateId = plan.TemplateId,
				TemplateName = plan.Template?.Name ?? "",
				IsPublished = plan.IsPublished,
				Slots = plan.SlotPlans
					.OrderBy(sp => sp.Slot.SortOrder)
					.ThenBy(sp => sp.Slot.StartTime)
					.Select(sp => new DayPlanSlotDTO
					{
						SlotId = sp.SlotId,
						StartTime = sp.Slot.StartTime,
						EndTime = sp.Slot.EndTime,

						EmployeeId = sp.EmployeeId,
						EmployeeName = sp.Employee != null ? sp.Employee.DisplayName : null,
						EmployeeColorHex = sp.Employee != null ? sp.Employee.ProfileColorHex : null,
						EmployeeHasAvatar = sp.Employee != null
							&& sp.Employee.AvatarImage != null
							&& sp.Employee.AvatarImage.Length > 0
					})
					.ToList()
			};
		}
	}
}
