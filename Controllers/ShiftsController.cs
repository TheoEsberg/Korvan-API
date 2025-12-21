using Korvan_API.Data;
using Korvan_API.Entities;
using Korvan_API.Extensions;
using Korvan_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Korvan_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShiftsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/shifts?from=(startdate)&to=(enddate)
        // list of all shifts in range
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WorkShiftDTO>>> GetShifts(
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to)
        {
            var query = _context.WorkShifts
                .Include(s => s.Employee)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(s => s.ShiftDate >= from.Value);
            if (to.HasValue)
                query = query.Where(s => s.ShiftDate <= to.Value);

            var shifts = await query
                .OrderBy(s => s.ShiftDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new WorkShiftDTO
                {
                    Id = s.Id,
                    ShiftDate = s.ShiftDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Notes = s.Notes,
                    Status = s.Status,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee != null ? s.Employee.DisplayName : null,
					EmployeeColorHex = s.Employee != null ? s.Employee.ProfileColorHex : null,
					EmployeeAvatarUrl = s.Employee != null ? s.Employee.AvatarUrl : null,
					CreatedById = s.CreatedById,
                    CreatedAt = s.CreatedAt,
                    UpdatedById = s.UpdatedById,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            return Ok(shifts);
        }

        // GET: api/shifts/me
        // Employee: get only thier own shifts
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WorkShiftDTO>>> GetMyShifts(
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to)
        {
            var userId = User.GetUserId();

            var query = _context.WorkShifts
                .Include(s => s.Employee)
                .Where(s => s.EmployeeId == userId);

            if (from.HasValue)
                query = query.Where(s => s.ShiftDate >= from.Value);
            if (to.HasValue)
                query = query.Where(s => s.ShiftDate <= to.Value);

            var shifts = await query
                .OrderBy(s => s.ShiftDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new WorkShiftDTO
                {
                    Id = s.Id,
                    ShiftDate = s.ShiftDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Notes = s.Notes,
                    Status = s.Status,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee != null ? s.Employee.DisplayName : null,
					EmployeeColorHex = s.Employee != null ? s.Employee.ProfileColorHex : null,
					EmployeeAvatarUrl = s.Employee != null ? s.Employee.AvatarUrl : null,
					CreatedById = s.CreatedById,
                    CreatedAt = s.CreatedAt,
                    UpdatedById = s.UpdatedById,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            return Ok(shifts);
        }

        // GET: api/shifts/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<WorkShiftDTO>> GetShiftById(Guid id)
        {
            var shift = await _context.WorkShifts
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shift == null) return NotFound();

            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && shift.EmployeeId != userId)
                return Forbid();

            var dto = new WorkShiftDTO
            {
                Id = shift.Id,
                ShiftDate = shift.ShiftDate,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Notes = shift.Notes,
                Status = shift.Status,
                EmployeeId = shift.EmployeeId,
                EmployeeName = shift.Employee != null ? shift.Employee.DisplayName : null,
				EmployeeColorHex = shift.Employee != null ? shift.Employee.ProfileColorHex : null,
				EmployeeAvatarUrl = shift.Employee != null ? shift.Employee.AvatarUrl : null,
				CreatedById = shift.CreatedById,
                CreatedAt = shift.CreatedAt,
                UpdatedById = shift.UpdatedById,
                UpdatedAt = shift.UpdatedAt
            };

            return Ok(dto);
        }

        // POST: api/shifts
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WorkShiftDTO>> CreateShift([FromBody] CreateShiftDTO dto)
        {
            var creatorId = User.GetUserId();

            var shift = new WorkShift
            {
                ShiftDate = dto.ShiftDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Notes = dto.Notes,
                Status = ShiftStatus.Published,
                EmployeeId = dto.EmployeeId,
                CreatedById = creatorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkShifts.Add(shift);
            await _context.SaveChangesAsync();

            // reload with employee to build DTO
            await _context.Entry(shift).Reference(s => s.Employee).LoadAsync();

            var result = new WorkShiftDTO
            {
                Id = shift.Id,
                ShiftDate = shift.ShiftDate,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Notes = shift.Notes,
                Status = shift.Status,
                EmployeeId = shift.EmployeeId,
                EmployeeName = shift.Employee?.DisplayName,
				EmployeeColorHex = shift.Employee != null ? shift.Employee.ProfileColorHex : null,
				EmployeeAvatarUrl = shift.Employee != null ? shift.Employee.AvatarUrl : null,
				CreatedById = shift.CreatedById,
                CreatedAt = shift.CreatedAt
            };

            return CreatedAtAction(nameof(GetShiftById), new { id = shift.Id }, result);
        }

        // PUT: api/shifts/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateShift(Guid id, [FromBody] UpdateShiftDTO dto)
        {
            var shift = await _context.WorkShifts.FindAsync(id);
            if (shift == null) return NotFound();

            var updaterId = User.GetUserId();

            shift.ShiftDate = dto.ShiftDate;
            shift.StartTime = dto.StartTime;
            shift.EndTime = dto.EndTime;
            shift.Notes = dto.Notes;
            shift.EmployeeId = dto.EmployeeId;
            shift.Status = dto.Status;
            shift.UpdatedById = updaterId;
            shift.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/shifts/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShift(Guid id)
        {
            var shift = await _context.WorkShifts.FindAsync(id);
            if (shift == null) return NotFound();

            _context.WorkShifts.Remove(shift);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
