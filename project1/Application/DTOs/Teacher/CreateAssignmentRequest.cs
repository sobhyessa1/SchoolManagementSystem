using System;

namespace project1.Application.DTOs.Teacher
{
    public class CreateAssignmentRequest
    {
        public Guid ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
