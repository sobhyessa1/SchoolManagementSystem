using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project1.Application.DTOs.Teacher;
using project1.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project1.Controllers.Teacher
{
    [ApiController]
    [Route("api/teacher/[controller]")]
    [Authorize(Roles = "Teacher")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Mark([FromBody] AttendanceMarkRequest request)
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(sub, out var teacherId)) return Forbid();

            try
            {
                await _service.MarkAttendanceAsync(request, teacherId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
