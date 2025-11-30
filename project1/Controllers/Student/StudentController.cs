using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Infrastructure.Data;
using project1.Application.Interfaces;
using project1.Application.Helpers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project1.Controllers.Student
{
    [ApiController]
    [Route("api/student/[controller]")]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly SchoolDbContext _db;
        private readonly IClassService _classService;
        private readonly ISubmissionService _submissionService;

        public StudentController(SchoolDbContext db, IClassService classService, ISubmissionService submissionService)
        {
            _db = db;
            _classService = classService;
            _submissionService = submissionService;
        }

        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses(int page = 1, int pageSize = 10)
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(sub, out var studentId)) return Forbid();

            var result = await _classService.GetClassesForStudentAsync(studentId, page, pageSize);
            PaginationHelper.AddPaginationHeader(Response, result);
            return Ok(result);
        }

        [HttpGet("attendance")]
        public async Task<IActionResult> GetAttendance(Guid? classId = null, DateTime? from = null, DateTime? to = null)
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(sub, out var studentId)) return Forbid();

            var query = _db.Attendances.AsNoTracking().Where(a => a.StudentId == studentId);
            if (classId != null) query = query.Where(a => a.ClassId == classId);
            if (from != null) query = query.Where(a => a.Date >= from.Value.Date);
            if (to != null) query = query.Where(a => a.Date <= to.Value.Date);

            var items = await query.OrderByDescending(a => a.Date).ToListAsync();

            var result = items.Select(a => new {
                a.Id,
                a.ClassId,
                a.StudentId,
                Date = a.Date,
                Status = a.Status.ToString(),
                a.MarkedByTeacherId,
                a.CreatedDate
            });

            return Ok(result);
        }

        [HttpGet("grades")]
        public async Task<IActionResult> GetGrades(Guid? assignmentId = null)
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(sub, out var studentId)) return Forbid();

            var query = _db.Submissions
                .AsNoTracking()
                .Where(s => s.StudentId == studentId && s.Grade.HasValue);

            if (assignmentId.HasValue)
            {
                query = query.Where(s => s.AssignmentId == assignmentId.Value);
            }

            var items = await query
                .Include(s => s.Assignment)
                .OrderByDescending(s => s.SubmittedDate)
                .ToListAsync();

            var result = items.Select(s => new {
                s.Id,
                s.AssignmentId,
                AssignmentTitle = s.Assignment.Title,
                s.SubmittedDate,
                s.Grade,
                s.Remarks,
                GradedByTeacherId = s.GradedByTeacherId
            });

            return Ok(result);
        }
    }
}
