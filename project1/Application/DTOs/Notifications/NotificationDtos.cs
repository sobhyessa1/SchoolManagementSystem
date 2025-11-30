using System;

namespace project1.Application.DTOs.Notifications
{
    public class CreateNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RecipientRole { get; set; } // "Admin", "Teacher", "Student", or null for specific user
        public Guid? RecipientId { get; set; } // Specific user ID, or null if role-based
        public Guid? ClassId { get; set; } // Optional: Send to all students in a class
    }

    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RecipientRole { get; set; }
        public Guid? RecipientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class MarkNotificationReadRequest
    {
        public Guid NotificationId { get; set; }
    }
}
