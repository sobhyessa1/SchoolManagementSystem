using System;
using System.Collections.Generic;

namespace project1.Domain.Entities
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
