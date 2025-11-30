using Microsoft.EntityFrameworkCore;
using project1.Application.DTOs.Admin;
using project1.Domain.Entities;
using project1.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project1.Infrastructure.Repositories
{
    public class DepartmentRepository
    {
        private readonly SchoolDbContext _db;

        public DepartmentRepository(SchoolDbContext db)
        {
            _db = db;
        }

        public IQueryable<Department> Query() => _db.Departments.AsNoTracking();

        public async Task<Department?> GetByIdAsync(Guid id) => await _db.Departments.FindAsync(id);

        public async Task AddAsync(Department d)
        {
            _db.Departments.Add(d);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department d)
        {
            _db.Departments.Update(d);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Department d)
        {
            d.UpdatedDate = DateTime.UtcNow;
            // No IsActive on Department currently; implement if needed
            _db.Departments.Update(d);
            await _db.SaveChangesAsync();
        }
    }
}
