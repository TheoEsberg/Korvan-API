using Korvan_API.Data;
using Korvan_API.Entities;
using Korvan_API.Extensions;
using Korvan_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Korvan_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All of the endpoints require authentication
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/notifications/me?unreadOnly=true&take=50
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetMyNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int take = 50)
        {
            var userId = User.GetUserId();

            if (take <= 0) take = 50;
            if (take > 200) take = 200; // simple guard

            var query = _context.Notifications
                .Where(n => n.User.Id == userId);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Type = n.Type,
                    Message = n.Message,
                    RelatedShiftId = n.RelatedShiftId,
                    RelatedOfferId = n.RelatedOfferId,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // GET api/notifications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDTO>> GetNotificationById(Guid id)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);

            if (n == null) return NotFound();

            // Employee can only see own notifications
            if (!isAdmin && n.User.Id != userId)
                return Forbid();

            var dto = new NotificationDTO
            {
                Id = n.Id,
                Type = n.Type,
                Message = n.Message,
                RelatedShiftId = n.RelatedShiftId,
                RelatedOfferId = n.RelatedOfferId,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            };

            return Ok(dto);
        }

        // PATCH: api/notifications/{id}
        // Mark a single notification as read/unread
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateNotificationStatus(
            Guid id,
            [FromBody] UpdateNotificationStatusDTO dto)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (n == null) return NotFound();

            if (!isAdmin && n.User.Id != userId)
                return Forbid();

            n.IsRead = dto.IsRead;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/notifications/mark-all-read
        // Mark all current user's notifications as read
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.GetUserId();

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // OPTIONAL: For testing / admin manual notifications
        // POST: api/notifications
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationDTO>> CreateNotificationForUser(
            Guid userId,
            [FromBody] string message)
        {
            // Simple manual notification creation for testing/admin purposes
            var notification = new Notification
            {
                UserId = userId,
                Type = NotificationType.ShiftChanged,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var dto = new NotificationDTO
            {
                Id = notification.Id,
                Type = notification.Type,
                Message = notification.Message,
                RelatedShiftId = notification.RelatedShiftId,
                RelatedOfferId = notification.RelatedOfferId,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };

            return CreatedAtAction(nameof(GetNotificationById), new { id = dto.Id }, dto);
        }
    }
}
