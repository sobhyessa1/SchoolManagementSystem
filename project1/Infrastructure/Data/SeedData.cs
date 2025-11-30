using project1.Domain.Entities;
using project1.Domain.Enums;
using System;
using System.Linq;

namespace project1.Infrastructure.Data
{
    public static class SeedData
    {
        public static void EnsureSeedData(SchoolDbContext db)
        {
            if (!db.Users.Any())
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin User",
                    Email = "admin@school.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = Role.Admin,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                db.Users.Add(admin);

                var teacher = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Teacher One",
                    Email = "teacher1@school.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                    Role = Role.Teacher,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                db.Users.Add(teacher);

                var student = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Student One",
                    Email = "student1@school.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                    Role = Role.Student,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                db.Users.Add(student);

                db.SaveChanges();
            }

            if (!db.Departments.Any())
            {
                var d = new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Computer Science",
                    Description = "CS Department",
                    CreatedDate = DateTime.UtcNow
                };
                db.Departments.Add(d);
                db.SaveChanges();
            }

            if (!db.Courses.Any())
            {
                var dept = db.Departments.First();
                var c = new Course
                {
                    Id = Guid.NewGuid(),
                    Name = "Intro to Programming",
                    Code = "CS101",
                    DepartmentId = dept.Id,
                    Credits = 3,
                    CreatedDate = DateTime.UtcNow
                };
                db.Courses.Add(c);
                db.SaveChanges();
            }

            if (!db.Classes.Any())
            {
                var course = db.Courses.First();
                var teacher = db.Users.First(u => u.Role == Role.Teacher);
                var cls = new Class
                {
                    Id = Guid.NewGuid(),
                    Name = "CS101 - Fall",
                    CourseId = course.Id,
                    TeacherId = teacher.Id,
                    Semester = 1,
                    StartDate = DateTime.UtcNow.AddDays(-7),
                    EndDate = DateTime.UtcNow.AddMonths(3),
                    CreatedDate = DateTime.UtcNow
                };
                db.Classes.Add(cls);
                db.SaveChanges();
            }
        }
    }
}
