using AutoMapper;
using project1.Application.DTOs.Admin;
using project1.Application.DTOs;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? filter)
        {
            Expression<Func<User, bool>>? filterExpr = null;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filterExpr = u => u.Name.Contains(filter) || u.Email.Contains(filter);
            }

            var total = await _unitOfWork.Repository<User>().CountAsync(filterExpr);
            var items = await _unitOfWork.Repository<User>().GetPagedAsync(
                page, 
                pageSize, 
                filterExpr, 
                q => q.OrderByDescending(u => u.CreatedDate)
            );

            return new PagedResult<UserDto> 
            { 
                Page = page, 
                PageSize = pageSize, 
                Total = total, 
                Items = items.Select(u => _mapper.Map<UserDto>(u)) 
            };
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var u = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            return u == null ? null : _mapper.Map<UserDto>(u);
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            var exists = await _unitOfWork.Repository<User>().ExistsAsync(u => u.Email == request.Email);
            if (exists) throw new InvalidOperationException("Email already in use");

            // Hash password
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
            }

            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Name = request.Name, 
                Email = request.Email, 
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = request.Role, 
                IsActive = true, 
                CreatedDate = DateTime.UtcNow 
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(Guid id, CreateUserRequest request)
        {
            var u = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (u == null) throw new InvalidOperationException("User not found");

            u.Name = request.Name;
            u.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Password)) 
            {
                using var hmac = new System.Security.Cryptography.HMACSHA512();
                u.PasswordSalt = hmac.Key;
                u.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
            }

            _unitOfWork.Repository<User>().Update(u);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var u = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (u == null) throw new InvalidOperationException("User not found");

            _unitOfWork.Repository<User>().Delete(u);
            await _unitOfWork.CompleteAsync();
        }
    }
}
