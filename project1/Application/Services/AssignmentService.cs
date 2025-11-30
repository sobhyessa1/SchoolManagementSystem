using AutoMapper;
using project1.Application.DTOs;
using project1.Application.DTOs.Teacher;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AssignmentDto>> GetAssignmentsForClassAsync(Guid classId, int page, int pageSize)
        {
            var total = await _unitOfWork.Repository<Assignment>().CountAsync(a => a.ClassId == classId);
            var items = await _unitOfWork.Repository<Assignment>().GetPagedAsync(
                page, 
                pageSize, 
                a => a.ClassId == classId, 
                q => q.OrderByDescending(a => a.CreatedDate)
            );

            return new PagedResult<AssignmentDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items.Select(a => _mapper.Map<AssignmentDto>(a))
            };
        }

        public async Task<AssignmentDto> CreateAsync(CreateAssignmentRequest request, Guid teacherId)
        {
            if (request.DueDate <= DateTime.UtcNow) throw new InvalidOperationException("Assignment due date must be in the future");

            var cls = await _unitOfWork.Repository<Class>().GetByIdAsync(request.ClassId);
            if (cls == null) throw new InvalidOperationException("Class not found");
            if (cls.TeacherId != teacherId) throw new UnauthorizedAccessException("Only assigned teacher can create assignment for this class");

            var assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                ClassId = request.ClassId,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                CreatedDate = DateTime.UtcNow,
                CreatedByTeacherId = teacherId
            };

            await _unitOfWork.Repository<Assignment>().AddAsync(assignment);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AssignmentDto>(assignment);
        }

        public async Task GradeSubmissionAsync(Guid submissionId, decimal grade, Guid teacherId, string? remarks)
        {
            var submissions = await _unitOfWork.Repository<Submission>().GetPagedAsync(
                1, 1, 
                s => s.Id == submissionId, 
                null, 
                includeProperties: "Assignment"
            );
            var submission = submissions.FirstOrDefault();

            if (submission == null) throw new InvalidOperationException("Submission not found");
            
            var cls = await _unitOfWork.Repository<Class>().GetByIdAsync(submission.Assignment.ClassId);
            if (cls == null) throw new InvalidOperationException("Class not found");
            if (cls.TeacherId != teacherId) throw new UnauthorizedAccessException("Only assigned teacher can grade this submission");

            submission.Grade = grade;
            submission.GradedByTeacherId = teacherId;
            submission.Remarks = remarks;

            _unitOfWork.Repository<Submission>().Update(submission);
            await _unitOfWork.CompleteAsync();
        }
    }
}
