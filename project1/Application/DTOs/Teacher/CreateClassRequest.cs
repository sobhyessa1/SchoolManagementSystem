using System;

namespace project1.Application.DTOs.Teacher
{
    public class CreateClassRequest
    {
        public string Name { get; set; } = null!;
        public Guid CourseId { get; set; }
        public Guid TeacherId { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
