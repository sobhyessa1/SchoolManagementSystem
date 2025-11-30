using System;
using project1.Domain.Enums;

namespace project1.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Role Role { get; set; }
    }
}
