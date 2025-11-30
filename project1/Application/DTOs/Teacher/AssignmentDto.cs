using System;

namespace project1.Application.DTOs.Teacher
{
    public class AssignmentDto
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedByTeacherId { get; set; }
    }
}
