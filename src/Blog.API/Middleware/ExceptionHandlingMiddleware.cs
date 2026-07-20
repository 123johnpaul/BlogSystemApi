using System.Net;
using Blog.API.Models;
using FluentValidation;

namespace Blog.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) { _next = next; _logger = logger; }
    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled request failure for {Path}", context.Request.Path);
            var (status, message, errors) = exception switch
            {
                ValidationException validation => ((int)HttpStatusCode.BadRequest, "Validation failed.", validation.Errors.Select(x => x.ErrorMessage).ToArray()),
                UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, "You are not authorized to perform this action.", Array.Empty<string>()),
                InvalidOperationException invalidOperation => ((int)HttpStatusCode.BadRequest, invalidOperation.Message, Array.Empty<string>()),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.", Array.Empty<string>())
            };
            context.Response.StatusCode = status; context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new ApiResponse<object> { Success = false, Message = message, Errors = errors, Timestamp = DateTime.UtcNow });
        }
    }
}
