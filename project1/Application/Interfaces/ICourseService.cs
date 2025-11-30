using project1.Application.DTOs.Admin;
using System;
using System.Threading.Tasks;
using project1.Application.DTOs;

namespace project1.Application.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDto>> GetCoursesAsync(int page, int pageSize, string? filter, Guid? departmentId);
        Task<CourseDto?> GetByIdAsync(Guid id);
        Task<CourseDto> CreateAsync(CreateCourseRequest request);
        Task UpdateAsync(Guid id, CreateCourseRequest request);
        Task DeleteAsync(Guid id);
    }
}
