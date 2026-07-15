using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Services;

public interface IOtpService
{
    Task<Otp> GenerateOtpAsync(User user, OtpPurpose purpose);

    Task<bool> VerifyOtpAsync(User user, string code, OtpPurpose purpose);
}