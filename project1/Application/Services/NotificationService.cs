using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project1.Application.DTOs.Notifications;
using project1.Application.Interfaces;
using project1.Domain.Entities;

namespace project1.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request, Guid createdByUserId)
        {
            var notifications = new List<Notification>();

            // If ClassId is specified, send to all students in that class
            if (request.ClassId.HasValue)
            {
                var studentClasses = await _unitOfWork.Repository<StudentClass>()
                    .FindAsync(sc => sc.ClassId == request.ClassId.Value);
                
                var studentIds = studentClasses.Select(sc => sc.StudentId).ToList();

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

                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Role == roleEnum);
                
                var userIds = users.Select(u => u.Id).ToList();

                foreach (var userId in userIds)
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
                foreach (var notification in notifications)
                {
                    await _unitOfWork.Repository<Notification>().AddAsync(notification);
                }
                await _unitOfWork.CompleteAsync();

                // Return the first notification as response (for single notifications)
                // For bulk notifications, this represents the template
                var first = notifications.First();
                var creator = await _unitOfWork.Repository<User>().GetByIdAsync(createdByUserId);
                
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
            var notifications = unreadOnly
                ? await _unitOfWork.Repository<Notification>().GetPagedAsync(
                    1, 1000, 
                    n => n.RecipientId == userId && !n.IsRead, 
                    q => q.OrderByDescending(n => n.CreatedDate),
                    "CreatedBy")
                : await _unitOfWork.Repository<Notification>().GetPagedAsync(
                    1, 1000, 
                    n => n.RecipientId == userId, 
                    q => q.OrderByDescending(n => n.CreatedDate),
                    "CreatedBy");

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
            var notifications = await _unitOfWork.Repository<Notification>()
                .FindAsync(n => n.Id == notificationId && n.RecipientId == userId);
            
            var notification = notifications.FirstOrDefault();

            if (notification == null)
                return false;

            notification.IsRead = true;
            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .FindAsync(n => n.RecipientId == userId && !n.IsRead);

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _unitOfWork.Repository<Notification>().Update(notification);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var count = await _unitOfWork.Repository<Notification>()
                .CountAsync(n => n.RecipientId == userId && !n.IsRead);
            return (int)count;
        }
    }
}
