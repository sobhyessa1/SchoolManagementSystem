using project1.Application.DTOs.Teacher;
using System.Threading.Tasks;

namespace project1.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task MarkAttendanceAsync(AttendanceMarkRequest request, System.Guid teacherId);
    }
}
