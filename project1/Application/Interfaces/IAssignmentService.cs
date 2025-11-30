using project1.Application.DTOs.Teacher;
using System;
using System.Threading.Tasks;
using project1.Application.DTOs;

namespace project1.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<PagedResult<AssignmentDto>> GetAssignmentsForClassAsync(Guid classId, int page, int pageSize);
        Task<AssignmentDto> CreateAsync(CreateAssignmentRequest request, Guid teacherId);
        Task GradeSubmissionAsync(Guid submissionId, decimal grade, Guid teacherId, string? remarks);
    }
}
