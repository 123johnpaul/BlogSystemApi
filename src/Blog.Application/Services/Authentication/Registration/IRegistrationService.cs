using Blog.Application.DTOs.Auth;

namespace Blog.Application.Services.Authentication.Registration;

public interface IRegistrationService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
}