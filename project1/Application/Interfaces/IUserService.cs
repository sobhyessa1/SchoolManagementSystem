using project1.Application.DTOs.Admin;
using System;
using System.Threading.Tasks;
using project1.Application.DTOs;

namespace project1.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? filter);
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto> CreateAsync(CreateUserRequest request);
        Task UpdateAsync(Guid id, CreateUserRequest request);
        Task DeleteAsync(Guid id);
    }
}
