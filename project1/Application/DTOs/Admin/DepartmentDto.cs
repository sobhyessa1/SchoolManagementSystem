using System;

namespace project1.Application.DTOs.Admin
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? HeadOfDepartmentId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
