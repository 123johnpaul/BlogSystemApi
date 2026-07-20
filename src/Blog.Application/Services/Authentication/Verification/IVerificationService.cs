using Blog.Application.DTOs.Auth;

namespace Blog.Application.Services.Authentication.Verification;

public interface IVerificationService
{
    Task VerifyEmailAsync(VerifyEmailRequest request);
}