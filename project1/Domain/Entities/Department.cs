using System;
using System.Collections.Generic;

namespace project1.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? HeadOfDepartmentId { get; set; }
        public User? HeadOfDepartment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
