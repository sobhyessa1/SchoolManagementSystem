using AutoMapper;
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
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<DepartmentDto>> GetDepartmentsAsync(int page, int pageSize, string? filter)
        {
            Expression<Func<Department, bool>>? filterExpr = null;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filterExpr = d => d.Name.Contains(filter);
            }

            var total = await _unitOfWork.Repository<Department>().CountAsync(filterExpr);
            var items = await _unitOfWork.Repository<Department>().GetPagedAsync(
                page, 
                pageSize, 
                filterExpr, 
                q => q.OrderByDescending(d => d.CreatedDate)
            );

            return new PagedResult<DepartmentDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items.Select(d => _mapper.Map<DepartmentDto>(d))
            };
        }

        public async Task<DepartmentDto?> GetByIdAsync(Guid id)
        {
            var d = await _unitOfWork.Repository<Department>().GetByIdAsync(id);
            return d == null ? null : _mapper.Map<DepartmentDto>(d);
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request)
        {
            var exists = await _unitOfWork.Repository<Department>().ExistsAsync(d => d.Name == request.Name);
            if (exists) throw new InvalidOperationException("Department name already exists");

            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                HeadOfDepartmentId = request.HeadOfDepartmentId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Repository<Department>().AddAsync(dept);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<DepartmentDto>(dept);
        }

        public async Task UpdateAsync(Guid id, CreateDepartmentRequest request)
        {
            var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(id);
            if (dept == null) throw new InvalidOperationException("Department not found");

            dept.Name = request.Name;
            dept.Description = request.Description;
            dept.HeadOfDepartmentId = request.HeadOfDepartmentId;
            
            _unitOfWork.Repository<Department>().Update(dept);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(id);
            if (dept == null) throw new InvalidOperationException("Department not found");

            _unitOfWork.Repository<Department>().Delete(dept);
            await _unitOfWork.CompleteAsync();
        }
    }
}
