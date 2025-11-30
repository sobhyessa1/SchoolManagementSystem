using project1.Application.DTOs.Teacher;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SubmitAsync(Guid assignmentId, Guid studentId, string? fileUrl)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(assignmentId);
            if (assignment == null) throw new InvalidOperationException("Assignment not found");
            if (assignment.DueDate < DateTime.UtcNow) throw new InvalidOperationException("Cannot submit after due date");

            // Prevent duplicate submission
            var exists = await _unitOfWork.Repository<Submission>().ExistsAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
            if (exists) throw new InvalidOperationException("Submission already exists");

            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                AssignmentId = assignmentId,
                StudentId = studentId,
                SubmittedDate = DateTime.UtcNow,
                FileUrl = fileUrl
            };

            await _unitOfWork.Repository<Submission>().AddAsync(submission);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<SubmissionDto[]> GetSubmissionsForAssignmentAsync(Guid assignmentId)
        {
            var items = await _unitOfWork.Repository<Submission>().FindAsync(s => s.AssignmentId == assignmentId);
            return items.Select(s => new SubmissionDto
            {
                Id = s.Id,
                AssignmentId = s.AssignmentId,
                StudentId = s.StudentId,
                SubmittedDate = s.SubmittedDate,
                FileUrl = s.FileUrl,
                Grade = s.Grade,
                GradedByTeacherId = s.GradedByTeacherId,
                Remarks = s.Remarks
            }).ToArray();
        }
    }
}
