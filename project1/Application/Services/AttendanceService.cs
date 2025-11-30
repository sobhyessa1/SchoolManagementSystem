using project1.Application.DTOs.Teacher;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task MarkAttendanceAsync(AttendanceMarkRequest request, Guid teacherId)
        {
            var cls = await _unitOfWork.Repository<Class>().GetByIdAsync(request.ClassId);
            if (cls == null) throw new InvalidOperationException("Class not found");
            if (cls.TeacherId != teacherId) throw new UnauthorizedAccessException("Only assigned teacher can mark attendance for this class");

            foreach (var entry in request.Entries)
            {
                var existingList = await _unitOfWork.Repository<Attendance>().FindAsync(a => a.ClassId == request.ClassId && a.StudentId == entry.StudentId && a.Date == request.Date.Date);
                var existing = existingList.FirstOrDefault();

                if (existing != null)
                {
                    existing.Status = entry.Status;
                    existing.MarkedByTeacherId = teacherId;
                    existing.CreatedDate = DateTime.UtcNow;
                    _unitOfWork.Repository<Attendance>().Update(existing);
                }
                else
                {
                    var a = new Attendance
                    {
                        Id = Guid.NewGuid(),
                        ClassId = request.ClassId,
                        StudentId = entry.StudentId,
                        Date = request.Date.Date,
                        Status = entry.Status,
                        MarkedByTeacherId = teacherId,
                        CreatedDate = DateTime.UtcNow
                    };
                    await _unitOfWork.Repository<Attendance>().AddAsync(a);
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
