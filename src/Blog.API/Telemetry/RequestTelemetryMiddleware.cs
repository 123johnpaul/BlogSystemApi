using System.Diagnostics;

namespace Blog.API.Telemetry;

public class RequestTelemetryMiddleware
{
    private static readonly ActivitySource ActivitySource = new("BlogSystem.Api");
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTelemetryMiddleware> _logger;

    public RequestTelemetryMiddleware(RequestDelegate next, ILogger<RequestTelemetryMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ActivitySource.StartActivity($"{context.Request.Method} {context.Request.Path}");

        await _next(context);

        _logger.LogInformation(
            "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
