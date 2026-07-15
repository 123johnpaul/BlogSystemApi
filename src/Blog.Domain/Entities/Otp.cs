using Blog.Domain.Enums;

namespace Blog.Domain.Entities;

public class Otp
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = string.Empty;

    public OtpPurpose Purpose { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}