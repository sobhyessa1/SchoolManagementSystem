using project1.Domain.Enums;

namespace project1.Application.DTOs.Admin
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Role Role { get; set; } = Role.Student;
    }
}
