using System;
using System.Collections.Generic;

namespace project1.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public int Credits { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
