using System;

namespace project1.Domain.Entities
{
    public class Submission
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
        public DateTime SubmittedDate { get; set; }
        public string? FileUrl { get; set; }
        public decimal? Grade { get; set; }
        public Guid? GradedByTeacherId { get; set; }
        public User? GradedByTeacher { get; set; }
        public string? Remarks { get; set; }
    }
}
