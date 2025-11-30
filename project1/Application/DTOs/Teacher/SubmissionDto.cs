using System;

namespace project1.Application.DTOs.Teacher
{
    public class SubmissionDto
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string? FileUrl { get; set; }
        public decimal? Grade { get; set; }
        public Guid? GradedByTeacherId { get; set; }
        public string? Remarks { get; set; }
    }
}
