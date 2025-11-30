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
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ClassDto>> GetClassesForTeacherAsync(Guid teacherId, int page, int pageSize)
        {
            var total = await _unitOfWork.Repository<Class>().CountAsync(c => c.TeacherId == teacherId);
            var items = await _unitOfWork.Repository<Class>().GetPagedAsync(
                page, 
                pageSize, 
                c => c.TeacherId == teacherId, 
                q => q.OrderByDescending(c => c.CreatedDate)
            );

            return new PagedResult<ClassDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items.Select(c => _mapper.Map<ClassDto>(c))
            };
        }

        public async Task<PagedResult<ClassDto>> GetClassesForStudentAsync(Guid studentId, int page, int pageSize)
        {
            // Query StudentClasses to find enrollments
            var total = await _unitOfWork.Repository<StudentClass>().CountAsync(sc => sc.StudentId == studentId);
            var items = await _unitOfWork.Repository<StudentClass>().GetPagedAsync(
                page, 
                pageSize, 
                sc => sc.StudentId == studentId, 
                q => q.OrderByDescending(sc => sc.EnrollmentDate),
                includeProperties: "Class"
            );

            return new PagedResult<ClassDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items.Select(sc => _mapper.Map<ClassDto>(sc.Class))
            };
        }

        public async Task<ClassDto?> GetByIdAsync(Guid id)
        {
            var c = await _unitOfWork.Repository<Class>().GetByIdAsync(id);
            return c == null ? null : _mapper.Map<ClassDto>(c);
        }

        public async Task<ClassDto> CreateAsync(CreateClassRequest request)
        {
            var cls = new Class
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CourseId = request.CourseId,
                TeacherId = request.TeacherId,
                Semester = request.Semester,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Repository<Class>().AddAsync(cls);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ClassDto>(cls);
        }

        public async Task UpdateAsync(Guid id, CreateClassRequest request)
        {
            var cls = await _unitOfWork.Repository<Class>().GetByIdAsync(id);
            if (cls == null) throw new InvalidOperationException("Class not found");

            cls.Name = request.Name;
            cls.CourseId = request.CourseId;
            cls.TeacherId = request.TeacherId;
            cls.Semester = request.Semester;
            cls.StartDate = request.StartDate;
            cls.EndDate = request.EndDate;
            
            _unitOfWork.Repository<Class>().Update(cls);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AssignStudentsAsync(Guid classId, Guid[] studentIds)
        {
            var cls = await _unitOfWork.Repository<Class>().GetByIdAsync(classId);
            if (cls == null) throw new InvalidOperationException("Class not found");

            foreach (var sid in studentIds)
            {
                var exists = await _unitOfWork.Repository<StudentClass>().ExistsAsync(sc => sc.ClassId == classId && sc.StudentId == sid);
                if (exists) continue;

                var sc = new StudentClass
                {
                    Id = Guid.NewGuid(),
                    ClassId = classId,
                    StudentId = sid,
                    EnrollmentDate = DateTime.UtcNow
                };
                await _unitOfWork.Repository<StudentClass>().AddAsync(sc);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
