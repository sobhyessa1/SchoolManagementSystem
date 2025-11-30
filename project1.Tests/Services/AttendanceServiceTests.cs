using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using project1.Application.Services;
using project1.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace project1.Tests.Services
{
    public class AttendanceServiceTests
    {
        private SchoolDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
            var db = new SchoolDbContext(options);
            db.Database.OpenConnection();
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async Task Teacher_Cannot_Mark_Attendance_For_Other_Class()
        {
            var db = CreateContext();
            // seed teacher, other teacher, department, course, class
            var t1 = new project1.Domain.Entities.User { Id = Guid.NewGuid(), Name = "T1", Email = "t1@x.com", PasswordHash = "x", Role = project1.Domain.Enums.Role.Teacher, CreatedDate = DateTime.UtcNow };
            var t2 = new project1.Domain.Entities.User { Id = Guid.NewGuid(), Name = "T2", Email = "t2@x.com", PasswordHash = "x", Role = project1.Domain.Enums.Role.Teacher, CreatedDate = DateTime.UtcNow };
            db.Users.AddRange(t1, t2);
            var dept = new project1.Domain.Entities.Department { Id = Guid.NewGuid(), Name = "D1", CreatedDate = DateTime.UtcNow };
            db.Departments.Add(dept);
            var course = new project1.Domain.Entities.Course { Id = Guid.NewGuid(), Name = "C", Code = "C1", DepartmentId = dept.Id, Credits = 3, CreatedDate = DateTime.UtcNow };
            db.Courses.Add(course);
            var cls = new project1.Domain.Entities.Class { Id = Guid.NewGuid(), Name = "CLS", CourseId = course.Id, TeacherId = t1.Id, Semester = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(1), CreatedDate = DateTime.UtcNow };
            db.Classes.Add(cls);
            await db.SaveChangesAsync();

            var svc = new AttendanceService(db);
            var req = new project1.Application.DTOs.Teacher.AttendanceMarkRequest { ClassId = cls.Id, Date = DateTime.UtcNow, Entries = new[] { new project1.Application.DTOs.Teacher.AttendanceEntry { StudentId = Guid.NewGuid(), Status = project1.Domain.Enums.AttendanceStatus.Present } } };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.MarkAttendanceAsync(req, t2.Id));
        }
    }
}
