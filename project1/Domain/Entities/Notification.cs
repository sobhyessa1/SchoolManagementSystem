using System;

namespace project1.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RecipientRole { get; set; } // Admin, Teacher, Student, or null for specific user
        public Guid? RecipientId { get; set; } // Specific user ID, or null if role-based
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public Guid CreatedByUserId { get; set; }

        // Navigation properties
        public User? Recipient { get; set; }
        public User? CreatedBy { get; set; }
    }
}
