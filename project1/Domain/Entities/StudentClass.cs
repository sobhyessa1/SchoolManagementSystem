using System;

namespace project1.Domain.Entities
{
    public class StudentClass
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
    }
}
