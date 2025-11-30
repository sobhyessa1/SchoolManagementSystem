namespace project1.Application.DTOs.Admin
{
    public class CreateDepartmentRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public System.Guid? HeadOfDepartmentId { get; set; }
    }
}
