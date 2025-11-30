using System;

namespace project1.Application.DTOs.Teacher
{
    public class AttendanceMarkRequest
    {
        public Guid ClassId { get; set; }
        public DateTime Date { get; set; }
        public AttendanceEntry[] Entries { get; set; } = Array.Empty<AttendanceEntry>();
    }

    public class AttendanceEntry
    {
        public Guid StudentId { get; set; }
        public project1.Domain.Enums.AttendanceStatus Status { get; set; }
    }
}
