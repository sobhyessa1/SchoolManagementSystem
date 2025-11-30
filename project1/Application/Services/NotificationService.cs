using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using project1.Application.DTOs.Notifications;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using project1.Infrastructure.Data;

namespace project1.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SchoolDbContext _context;

        public NotificationService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request, Guid createdByUserId)
        {
            var notifications = new List<Notification>();

            // If ClassId is specified, send to all students in that class
            if (request.ClassId.HasValue)
            {
                var studentIds = await _context.StudentClasses
                    .Where(sc => sc.ClassId == request.ClassId.Value)
                    .Select(sc => sc.StudentId)
                    .ToListAsync();

                foreach (var studentId in studentIds)
                {
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        Title = request.Title,
                        Message = request.Message,
                        RecipientId = studentId,
                        RecipientRole = null,
                        CreatedDate = DateTime.UtcNow,
                        IsRead = false,
                        CreatedByUserId = createdByUserId
                    });
                }
            }
            // If RecipientId is specified, send to specific user
            else if (request.RecipientId.HasValue)
            {
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Message = request.Message,
                    RecipientId = request.RecipientId.Value,
                    RecipientRole = null,
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false,
                    CreatedByUserId = createdByUserId
                });
            }
            // If RecipientRole is specified, send to all users with that role
            else if (!string.IsNullOrWhiteSpace(request.RecipientRole))
            {
                // Parse the role string to enum
                if (!Enum.TryParse<Domain.Enums.Role>(request.RecipientRole, out var roleEnum))
                {
                    throw new ArgumentException($"Invalid role: {request.RecipientRole}");
                }

                var users = await _context.Users
                    .Where(u => u.Role == roleEnum)
                    .Select(u => u.Id)
                    .ToListAsync();

                foreach (var userId in users)
                {
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        Title = request.Title,
                        Message = request.Message,
                        RecipientId = userId,
                        RecipientRole = request.RecipientRole,
                        CreatedDate = DateTime.UtcNow,
                        IsRead = false,
                        CreatedByUserId = createdByUserId
                    });
                }
            }

            if (notifications.Any())
            {
                await _context.Notifications.AddRangeAsync(notifications);
                await _context.SaveChangesAsync();

                // Return the first notification as response (for single notifications)
                // For bulk notifications, this represents the template
                var first = notifications.First();
                var creator = await _context.Users.FindAsync(createdByUserId);
                
                return new NotificationResponse
                {
                    Id = first.Id,
                    Title = first.Title,
                    Message = first.Message,
                    RecipientRole = first.RecipientRole,
                    RecipientId = first.RecipientId,
                    CreatedDate = first.CreatedDate,
                    IsRead = first.IsRead,
                    CreatedByName = creator?.Name
                };
            }

            throw new InvalidOperationException("No notifications were created");
        }

        public async Task<List<NotificationResponse>> GetNotificationsForUserAsync(Guid userId, bool unreadOnly = false)
        {
            var query = _context.Notifications
                .Where(n => n.RecipientId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return notifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                RecipientRole = n.RecipientRole,
                RecipientId = n.RecipientId,
                CreatedDate = n.CreatedDate,
                IsRead = n.IsRead,
                CreatedByName = n.CreatedBy?.Name
            }).ToList();
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.RecipientId == userId && !n.IsRead);
        }
    }
}
