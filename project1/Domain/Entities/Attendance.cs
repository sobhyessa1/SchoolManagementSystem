using System;
using project1.Domain.Enums;

namespace project1.Domain.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public Guid MarkedByTeacherId { get; set; }
        public User MarkedByTeacher { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
