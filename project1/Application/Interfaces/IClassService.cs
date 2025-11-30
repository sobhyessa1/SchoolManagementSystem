using project1.Application.DTOs.Teacher;
using System;
using System.Threading.Tasks;
using project1.Application.DTOs;

namespace project1.Application.Interfaces
{
    public interface IClassService
    {
        Task<PagedResult<ClassDto>> GetClassesForTeacherAsync(Guid teacherId, int page, int pageSize);
        Task<PagedResult<ClassDto>> GetClassesForStudentAsync(Guid studentId, int page, int pageSize);
        Task<ClassDto?> GetByIdAsync(Guid id);
        Task<ClassDto> CreateAsync(CreateClassRequest request);
        Task UpdateAsync(Guid id, CreateClassRequest request);
        Task AssignStudentsAsync(Guid classId, Guid[] studentIds);
    }
}
