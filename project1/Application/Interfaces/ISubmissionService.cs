using project1.Application.DTOs.Teacher;
using System;
using System.Threading.Tasks;

namespace project1.Application.Interfaces
{
    public interface ISubmissionService
    {
        Task SubmitAsync(Guid assignmentId, Guid studentId, string? fileUrl);
        Task<SubmissionDto[]> GetSubmissionsForAssignmentAsync(Guid assignmentId);
    }
}
