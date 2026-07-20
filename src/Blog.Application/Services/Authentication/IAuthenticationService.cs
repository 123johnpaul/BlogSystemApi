using Blog.Application.DTOs.Auth;

namespace Blog.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    Task VerifyEmailAsync(VerifyEmailRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);

    Task LogoutAsync(Guid userId);

    Task ForgotPasswordAsync(ForgotPasswordRequest request);

    Task ResetPasswordAsync(ResetPasswordRequest request);

    Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request);
}
