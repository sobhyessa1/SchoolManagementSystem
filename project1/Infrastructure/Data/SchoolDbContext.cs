using Microsoft.EntityFrameworkCore;
using project1.Domain.Entities;

namespace project1.Infrastructure.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<StudentClass> StudentClasses { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.Email).IsRequired();
                b.Property(u => u.Name).IsRequired();
                b.HasQueryFilter(u => u.IsActive);
            });

            // RefreshToken
            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasOne(rt => rt.User).WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // Department
            modelBuilder.Entity<Department>(b =>
            {
                b.HasIndex(d => d.Name).IsUnique();
                b.HasOne(d => d.HeadOfDepartment)
                    .WithMany()
                    .HasForeignKey(d => d.HeadOfDepartmentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Course
            modelBuilder.Entity<Course>(b =>
            {
                b.HasIndex(c => new { c.DepartmentId, c.Code }).IsUnique();
                b.HasOne(c => c.Department).WithMany(d => d.Courses).HasForeignKey(c => c.DepartmentId);
                b.HasQueryFilter(c => c.IsActive);
                b.Property(c => c.Code).IsRequired().HasMaxLength(450);
            });

            // Class
            modelBuilder.Entity<Class>(b =>
            {
                b.HasOne(c => c.Course).WithMany(cu => cu.Classes).HasForeignKey(c => c.CourseId);
                b.HasOne(c => c.Teacher).WithMany(u => u.ClassesTeaching).HasForeignKey(c => c.TeacherId).OnDelete(DeleteBehavior.Restrict);
            });

            // StudentClass (unique constraint)
            modelBuilder.Entity<StudentClass>(b =>
            {
                b.HasIndex(sc => new { sc.StudentId, sc.ClassId }).IsUnique();
                b.HasOne(sc => sc.Student).WithMany(u => u.StudentClasses).HasForeignKey(sc => sc.StudentId);
                b.HasOne(sc => sc.Class).WithMany(c => c.StudentClasses).HasForeignKey(sc => sc.ClassId);
            });

            // Attendance composite unique
            modelBuilder.Entity<Attendance>(b =>
            {
                b.HasIndex(a => new { a.ClassId, a.StudentId, a.Date }).IsUnique();
                b.HasOne(a => a.Class).WithMany().HasForeignKey(a => a.ClassId);
                b.HasOne(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(a => a.MarkedByTeacher).WithMany().HasForeignKey(a => a.MarkedByTeacherId).OnDelete(DeleteBehavior.Restrict);
            });

            // Assignment
            modelBuilder.Entity<Assignment>(b =>
            {
                b.HasOne(a => a.Class).WithMany(c => c.Assignments).HasForeignKey(a => a.ClassId);
                b.HasOne(a => a.CreatedByTeacher).WithMany().HasForeignKey(a => a.CreatedByTeacherId).OnDelete(DeleteBehavior.Restrict);
            });

            // Submission
            modelBuilder.Entity<Submission>(b =>
            {
                b.HasOne(s => s.Assignment).WithMany(a => a.Submissions).HasForeignKey(s => s.AssignmentId);
                b.HasOne(s => s.Student).WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(s => s.GradedByTeacher).WithMany().HasForeignKey(s => s.GradedByTeacherId).OnDelete(DeleteBehavior.Restrict);

                // Ensure decimal precision for Grade to avoid truncation warnings
                b.Property(s => s.Grade).HasPrecision(18, 2);
            });

            // Notification
            modelBuilder.Entity<Notification>(b =>
            {
                b.HasOne(n => n.Recipient).WithMany().HasForeignKey(n => n.RecipientId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(n => n.CreatedBy).WithMany().HasForeignKey(n => n.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(n => new { n.RecipientId, n.IsRead });
                b.Property(n => n.Title).IsRequired().HasMaxLength(200);
                b.Property(n => n.Message).IsRequired();
            });
        }
    }
}
