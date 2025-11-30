using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using project1.Infrastructure.Data;
using project1.Domain.Entities;
using project1.Domain.Enums;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, conf) =>
    {
        conf.AddJsonFile("../project1/appsettings.Development.json", optional: false, reloadOnChange: false);
    })
    .ConfigureServices((ctx, services) =>
    {
        var conn = ctx.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<SchoolDbContext>(options => options.UseSqlServer(conn));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

// ensure created
db.Database.Migrate();

Console.WriteLine("Seeding 10 records per table...");

// Users
if (!db.Users.Any())
{
    var roles = new[] { Role.Admin, Role.Teacher, Role.Student };
    for (int i = 1; i <= 10; i++)
    {
        db.Users.Add(new User { Id = Guid.NewGuid(), Name = $"User{i}", Email = $"user{i}@example.test", PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd"), Role = roles[i % 3], IsActive = true, CreatedDate = DateTime.UtcNow });
    }
    db.SaveChanges();
}

// Departments
if (!db.Departments.Any())
{
    for (int i = 1; i <= 10; i++)
    {
        db.Departments.Add(new Department { Id = Guid.NewGuid(), Name = $"Dept{i}", Description = $"Department {i}", CreatedDate = DateTime.UtcNow });
    }
    db.SaveChanges();
}

// Courses
if (!db.Courses.Any())
{
    var deps = db.Departments.ToList();
    for (int i = 1; i <= 10; i++)
    {
        var dep = deps[(i - 1) % deps.Count];
        db.Courses.Add(new Course { Id = Guid.NewGuid(), Name = $"Course{i}", Code = $"C{i:000}", DepartmentId = dep.Id, Credits = 3, IsActive = true, CreatedDate = DateTime.UtcNow });
    }
    db.SaveChanges();
}

// Classes
if (!db.Classes.Any())
{
    var courses = db.Courses.ToList();
    var teachers = db.Users.Where(u => u.Role == Role.Teacher).ToList();
    for (int i = 1; i <= 10; i++)
    {
        var course = courses[(i - 1) % courses.Count];
        var teacher = teachers[(i - 1) % Math.Max(1, teachers.Count)];
        db.Classes.Add(new Class { Id = Guid.NewGuid(), Name = $"Class{i}", CourseId = course.Id, TeacherId = teacher.Id, Semester = 1, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddMonths(3), IsActive = true, CreatedDate = DateTime.UtcNow });
    }
    db.SaveChanges();
}

// StudentClasses
if (!db.StudentClasses.Any())
{
    var classes = db.Classes.ToList();
    var students = db.Users.Where(u => u.Role == Role.Student).ToList();
    int idx = 0;
    for (int i = 1; i <= 10; i++)
    {
        var st = students[idx % Math.Max(1, students.Count)];
        var cl = classes[idx % classes.Count];
        db.StudentClasses.Add(new StudentClass { Id = Guid.NewGuid(), StudentId = st.Id, ClassId = cl.Id, EnrollmentDate = DateTime.UtcNow });
        idx++;
    }
    db.SaveChanges();
}

// Assignments
if (!db.Assignments.Any())
{
    var classes = db.Classes.ToList();
    var teachers = db.Users.Where(u => u.Role == Role.Teacher).ToList();
    for (int i = 1; i <= 10; i++)
    {
        var cl = classes[(i - 1) % classes.Count];
        var t = teachers[(i - 1) % Math.Max(1, teachers.Count)];
        db.Assignments.Add(new Assignment { Id = Guid.NewGuid(), ClassId = cl.Id, Title = $"Assignment{i}", Description = "Seeded assignment", DueDate = DateTime.UtcNow.AddDays(7), CreatedDate = DateTime.UtcNow, CreatedByTeacherId = t.Id });
    }
    db.SaveChanges();
}

// Submissions
if (!db.Submissions.Any())
{
    var assignments = db.Assignments.ToList();
    var students = db.Users.Where(u => u.Role == Role.Student).ToList();
    for (int i = 1; i <= 10; i++)
    {
        var a = assignments[(i - 1) % assignments.Count];
        var s = students[(i - 1) % Math.Max(1, students.Count)];
        db.Submissions.Add(new Submission { Id = Guid.NewGuid(), AssignmentId = a.Id, StudentId = s.Id, SubmittedDate = DateTime.UtcNow, FileUrl = null, Remarks = "Seed submission" });
    }
    db.SaveChanges();
}

// Attendances
if (!db.Attendances.Any())
{
    var classes = db.Classes.ToList();
    var students = db.Users.Where(u => u.Role == Role.Student).ToList();
    var teacher = db.Users.FirstOrDefault(u => u.Role == Role.Teacher);
    for (int i = 1; i <= 10; i++)
    {
        var cl = classes[(i - 1) % classes.Count];
        var s = students[(i - 1) % Math.Max(1, students.Count)];
        db.Attendances.Add(new Attendance { Id = Guid.NewGuid(), ClassId = cl.Id, StudentId = s.Id, Date = DateTime.UtcNow.Date, Status = AttendanceStatus.Present, MarkedByTeacherId = teacher?.Id ?? Guid.Empty, CreatedDate = DateTime.UtcNow });
    }
    db.SaveChanges();
}

Console.WriteLine("Seeding complete.");
