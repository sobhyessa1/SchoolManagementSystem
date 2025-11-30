namespace project1.Application.DTOs.Admin
{
    public class CreateCourseRequest
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public System.Guid DepartmentId { get; set; }
        public int Credits { get; set; }
    }
}
