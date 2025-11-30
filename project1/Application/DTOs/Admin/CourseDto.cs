using System;

namespace project1.Application.DTOs.Admin
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public Guid DepartmentId { get; set; }
        public int Credits { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
