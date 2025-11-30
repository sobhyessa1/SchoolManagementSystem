using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project1.Application.DTOs.Admin;
using project1.Application.DTOs;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using project1.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class UserService : IUserService
    {
        private readonly SchoolDbContext _db;
        private readonly IMapper _mapper;

        public UserService(SchoolDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? filter)
        {
            var query = _db.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(filter)) query = query.Where(u => u.Name.Contains(filter) || u.Email.Contains(filter));
            var total = await query.LongCountAsync();
            var items = await query.OrderByDescending(u => u.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<UserDto> { Page = page, PageSize = pageSize, Total = total, Items = items.Select(u => _mapper.Map<UserDto>(u)) };
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var u = await _db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            return u == null ? null : _mapper.Map<UserDto>(u);
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (exists) throw new InvalidOperationException("Email already in use");

            var user = new User { Id = Guid.NewGuid(), Name = request.Name, Email = request.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), Role = request.Role, IsActive = true, CreatedDate = DateTime.UtcNow };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(Guid id, CreateUserRequest request)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) throw new InvalidOperationException("User not found");
            u.Name = request.Name;
            u.Email = request.Email;
            u.UpdatedDate = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Password)) u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            _db.Users.Update(u);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) throw new InvalidOperationException("User not found");
            u.IsActive = false;
            u.UpdatedDate = DateTime.UtcNow;
            _db.Users.Update(u);
            await _db.SaveChangesAsync();
        }
    }
}
