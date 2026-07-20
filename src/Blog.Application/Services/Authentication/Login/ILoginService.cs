using Blog.Application.DTOs.Auth;

namespace Blog.Application.Services.Authentication.Login;

public interface ILoginService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}