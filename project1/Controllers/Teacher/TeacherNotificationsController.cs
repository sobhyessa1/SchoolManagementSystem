using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project1.Application.DTOs.Notifications;
using project1.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project1.Controllers.Teacher
{
    [ApiController]
    [Route("api/teacher/notifications")]
    [Authorize(Roles = "Teacher")]
    public class TeacherNotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public TeacherNotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Create a notification (send to a specific student, a class, or all students)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var notification = await _notificationService.CreateNotificationAsync(request, userId);
            return CreatedAtAction(nameof(CreateNotification), new { id = notification.Id }, notification);
        }

        /// <summary>
        /// Get teacher's own notifications
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetNotificationsForUserAsync(userId, unreadOnly);
            return Ok(notifications);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var success = await _notificationService.MarkAsReadAsync(id, userId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            await _notificationService.MarkAllAsReadAsync(userId);
            return NoContent();
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }
    }
}
