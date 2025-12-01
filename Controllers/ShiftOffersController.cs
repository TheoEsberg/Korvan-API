using Korvan_API.Data;
using Korvan_API.Entities;
using Korvan_API.Extensions;
using Korvan_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Korvan_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftOffersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShiftOffersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/shiftoffers/pending
        [HttpGet("pending")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ShiftOfferDTO>>> GetPendingOffers()
        {
            var offers = await _context.ShiftOffers
                .Include(o => o.OfferedBy)
                .Include(o => o.TakenById)
                .Where(o => o.Status == ShiftOfferStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new ShiftOfferDTO
                {
                    Id = o.Id,
                    ShiftId = o.ShiftId,
                    OfferedById = o.OfferedById,
                    OfferedByName = o.OfferedBy.DisplayName,
                    TakenById = o.TakenById,
                    TakenByName = o.TakenBy != null ? o.TakenBy.DisplayName : null,
                    Type = o.Type,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    ResolvedAt = o.ResolvedAt
                })
                .ToListAsync();

            return Ok(offers);
        }

        // GET: api/shiftoffers/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ShiftOfferDTO>>> GetMyOffers()
        {
            var userId = User.GetUserId();

            var offers = await _context.ShiftOffers
                .Include(o => o.OfferedBy)
                .Include(o => o.TakenBy)
                .Where(o => o.OfferedById == userId || o.TakenById == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new ShiftOfferDTO
                {
                    Id = o.Id,
                    ShiftId = o.ShiftId,
                    OfferedById = o.OfferedById,
                    OfferedByName = o.OfferedBy.DisplayName,
                    TakenById = o.TakenById,
                    TakenByName = o.TakenBy != null ? o.TakenBy.DisplayName : null,
                    Type = o.Type,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    ResolvedAt = o.ResolvedAt
                })
                .ToListAsync();

            return Ok(offers);
        }

        // POST: api/shiftoffers/{shiftId}/offer
        [HttpPost("{shiftId:guid}/offer")]
        [Authorize]
        public async Task<ActionResult<ShiftOfferDTO>> OfferShift(Guid shiftId)
        {
            var userId = User.GetUserId();

            var shift = await _context.WorkShifts.FindAsync(shiftId);
            if (shift == null) return NotFound("Shift not found.");

            if (shift.EmployeeId != userId)
                return Forbid("You can only offer your own shifts.");

            if (shift.Status == ShiftStatus.Cancelled)
                return BadRequest("Cannot offer a cancelled shift.");

            // Mark shift as offered
            shift.Status = ShiftStatus.Offered;

            var offer = new ShiftOffer
            {
                ShiftId = shiftId,
                OfferedById = userId,
                Type = ShiftOfferType.Giveaway,
                Status = ShiftOfferStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.ShiftOffers.Add(offer);
            await _context.SaveChangesAsync();

            await _context.Entry(offer).Reference(o => o.OfferedBy).LoadAsync();

            var dto = new ShiftOfferDTO
            {
                Id = offer.Id,
                ShiftId = offer.ShiftId,
                OfferedById = offer.OfferedById,
                OfferedByName = offer.OfferedBy.DisplayName,
                TakenById = offer.TakenById,
                TakenByName = null,
                Type = offer.Type,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/shiftoffers/{offerId}/accept
        [HttpPost("{offerId:guid}/accept")]
        [Authorize]
        public async Task<ActionResult<ShiftOfferDTO>> AcceptOffer(Guid offerId)
        {
            var userId = User.GetUserId();

            var offer = await _context.ShiftOffers
                .Include(o => o.Shift)
                .Include(o => o.OfferedBy)
                .Include(o => o.TakenBy)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer == null) return NotFound("Offer not found.");
            if (offer.Status != ShiftOfferStatus.Pending)
                return BadRequest("Offer is no longer available.");
            if (offer.Shift.EmployeeId == userId)
                return BadRequest("You cannot accept your own shift offer.");

            // Assign shift to new user
            offer.Shift.EmployeeId = userId;
            offer.Shift.Status = ShiftStatus.Taken;

            offer.TakenById = userId;
            offer.Status = ShiftOfferStatus.Accepted;
            offer.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _context.Entry(offer).Reference(o => o.TakenBy).LoadAsync();

            var dto = new ShiftOfferDTO
            {
                Id = offer.Id,
                ShiftId = offer.ShiftId,
                OfferedById = offer.OfferedById,
                OfferedByName = offer.OfferedBy.DisplayName,
                TakenById = offer.TakenById,
                TakenByName = offer.TakenBy?.DisplayName,
                Type = offer.Type,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                ResolvedAt = offer.ResolvedAt
            };

            return Ok(dto);
        }

        // POST: api/shiftoffers/{offerId}/cancel
        [HttpPost("{offerId:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOffer(Guid offerId)
        {
            var userId = User.GetUserId();

            var offer = await _context.ShiftOffers
                .Include(o => o.Shift)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer == null) return NotFound("Offer not found.");
            if (offer.OfferedById != userId && !User.IsInRole("Admin"))
                return Forbid("You can only cancel your own shift offers.");

            if (offer.Status != ShiftOfferStatus.Pending)
                return BadRequest("Only pending offers can be cancelled."); 

            offer.Status = ShiftOfferStatus.Cancelled;
            offer.ResolvedAt = DateTime.UtcNow;

            // reset shift status if still offered
            if (offer.Shift.Status == ShiftStatus.Offered)
                offer.Shift.Status = ShiftStatus.Published;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
