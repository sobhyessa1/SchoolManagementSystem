using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using project1.Application.DTOs;
using project1.Application.DTOs.Admin;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CoursesCacheKey = "courses:all";

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResult<CourseDto>> GetCoursesAsync(int page, int pageSize, string? filter, Guid? departmentId)
        {
            // Try cache for unfiltered full list
            if (string.IsNullOrWhiteSpace(filter) && departmentId == null)
            {
                if (_cache.TryGetValue(CoursesCacheKey, out PagedResult<CourseDto> cached))
                {
                    return cached;
                }
            }

            Expression<Func<Course, bool>>? filterExpr = null;
            if (!string.IsNullOrWhiteSpace(filter) && departmentId != null)
            {
                filterExpr = c => (c.Name.Contains(filter) || c.Code.Contains(filter)) && c.DepartmentId == departmentId;
            }
            else if (!string.IsNullOrWhiteSpace(filter))
            {
                filterExpr = c => c.Name.Contains(filter) || c.Code.Contains(filter);
            }
            else if (departmentId != null)
            {
                filterExpr = c => c.DepartmentId == departmentId;
            }

            var total = await _unitOfWork.Repository<Course>().CountAsync(filterExpr);
            var items = await _unitOfWork.Repository<Course>().GetPagedAsync(
                page, 
                pageSize, 
                filterExpr, 
                q => q.OrderByDescending(c => c.CreatedDate),
                includeProperties: "Department"
            );

            var result = new PagedResult<CourseDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items.Select(c => _mapper.Map<CourseDto>(c))
            };

            if (string.IsNullOrWhiteSpace(filter) && departmentId == null)
            {
                _cache.Set(CoursesCacheKey, result, TimeSpan.FromMinutes(10));
            }

            return result;
        }

        public async Task<CourseDto?> GetByIdAsync(Guid id)
        {
            var c = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
            return c == null ? null : _mapper.Map<CourseDto>(c);
        }

        public async Task<CourseDto> CreateAsync(CreateCourseRequest request)
        {
            var exists = await _unitOfWork.Repository<Course>().ExistsAsync(c => c.DepartmentId == request.DepartmentId && c.Code == request.Code);
            if (exists) throw new InvalidOperationException("Course code already exists in department");

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                DepartmentId = request.DepartmentId,
                Credits = request.Credits,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Repository<Course>().AddAsync(course);
            await _unitOfWork.CompleteAsync();

            _cache.Remove(CoursesCacheKey);

            return _mapper.Map<CourseDto>(course);
        }

        public async Task UpdateAsync(Guid id, CreateCourseRequest request)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
            if (course == null) throw new InvalidOperationException("Course not found");

            // Ensure unique code per department
            var exists = await _unitOfWork.Repository<Course>().ExistsAsync(c => c.Id != id && c.DepartmentId == request.DepartmentId && c.Code == request.Code);
            if (exists) throw new InvalidOperationException("Course code already exists in department");

            course.Name = request.Name;
            course.Code = request.Code;
            course.Description = request.Description;
            course.DepartmentId = request.DepartmentId;
            course.Credits = request.Credits;
            
            _unitOfWork.Repository<Course>().Update(course);
            await _unitOfWork.CompleteAsync();

            _cache.Remove(CoursesCacheKey);
        }

        public async Task DeleteAsync(Guid id)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
            if (course == null) throw new InvalidOperationException("Course not found");

            _unitOfWork.Repository<Course>().Delete(course);
            await _unitOfWork.CompleteAsync();

            _cache.Remove(CoursesCacheKey);
        }
    }
}
