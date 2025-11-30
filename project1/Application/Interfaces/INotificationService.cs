using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using project1.Application.DTOs.Notifications;

namespace project1.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request, Guid createdByUserId);
        Task<List<NotificationResponse>> GetNotificationsForUserAsync(Guid userId, bool unreadOnly = false);
        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
        Task<bool> MarkAllAsReadAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}
