namespace Blog.API.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public IReadOnlyCollection<string>? Errors { get; init; }

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> Create(T? data, string message)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
}
