namespace Blog.Application.DTOs.Users;

public class UserProfileResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Bio { get; init; }
    public string? Avatar { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
}
