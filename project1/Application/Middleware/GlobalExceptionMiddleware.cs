using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace project1.Application.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = new ProblemDetails
            {
                Instance = context.Request.Path,
                Detail = exception.Message
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    problem.Status = (int)HttpStatusCode.Forbidden;
                    problem.Title = "Forbidden";
                    break;
                case InvalidOperationException:
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    problem.Title = "Bad Request";
                    break;
                default:
                    problem.Status = (int)HttpStatusCode.InternalServerError;
                    problem.Title = "An unexpected error occurred.";
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problem, options);
            return context.Response.WriteAsync(json);
        }
    }
}
