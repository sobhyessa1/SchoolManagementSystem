using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project1.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace project1.Controllers.Student
{
    [ApiController]
    [Route("api/student/[controller]")]
    [Authorize(Roles = "Student")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _service;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedExtensions = new[] { ".pdf", ".doc", ".docx", ".zip", ".txt" };

        public SubmissionsController(ISubmissionService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpPost("{assignmentId}/submit")]
        public async Task<IActionResult> Submit(Guid assignmentId, [FromForm] FileSubmitModel model)
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(sub, out var studentId)) return Forbid();

            string? fileUrl = model.FileUrl;
            if (model.File != null)
            {
                // Validate size
                if (model.File.Length > MaxFileBytes)
                    return BadRequest(new { error = "File too large. Max 5 MB allowed." });

                var ext = Path.GetExtension(model.File.FileName).ToLowerInvariant();
                if (Array.IndexOf(AllowedExtensions, ext) < 0)
                    return BadRequest(new { error = "File type not allowed." });

                var uploads = Path.Combine(_env.ContentRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await model.File.CopyToAsync(stream);
                }

                // Public URL served from /uploads/{fileName}
                fileUrl = $"/uploads/{fileName}";
            }

            try
            {
                await _service.SubmitAsync(assignmentId, studentId, fileUrl);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("assignment/{assignmentId}")]
        public async Task<IActionResult> GetForAssignment(Guid assignmentId)
        {
            var items = await _service.GetSubmissionsForAssignmentAsync(assignmentId);
            return Ok(items);
        }
    }

    public class FileSubmitModel
    {
        public IFormFile? File { get; set; }
        public string? FileUrl { get; set; }
    }
}
