using System;
using System.Collections.Generic;
using project1.Domain.Enums;

namespace project1.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public Role Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public ICollection<Class> ClassesTeaching { get; set; } = new List<Class>();
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    }
}
