using System;
using System.Collections.Generic;

namespace project1.Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedByTeacherId { get; set; }
        public User CreatedByTeacher { get; set; } = null!;

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
