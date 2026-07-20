using System.Security.Claims;
using Blog.API.Models;
using Blog.Application.DTOs.Auth;
using Blog.Application.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> RegisterAsync(RegisterRequest request)
    {
        var response = await _authenticationService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<RegisterResponse>.Create(response, response.Message));
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyEmailAsync(VerifyEmailRequest request)
    {
        await _authenticationService.VerifyEmailAsync(request);
        return Ok(ApiResponse<object>.Create(null, "Email verified successfully."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> LoginAsync(LoginRequest request)
    {
        var response = await _authenticationService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Create(response, "Login successful."));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var response = await _authenticationService.RefreshTokenAsync(request);
        return Ok(ApiResponse<LoginResponse>.Create(response, "Token refreshed successfully."));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _authenticationService.LogoutAsync(GetCurrentUserId());
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        await _authenticationService.ForgotPasswordAsync(request);
        return Ok(ApiResponse<object>.Create(null, "If an account exists for this email, a password reset code has been sent."));
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        await _authenticationService.ResetPasswordAsync(request);
        return Ok(ApiResponse<object>.Create(null, "Password reset successfully."));
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePasswordAsync(ChangePasswordRequest request)
    {
        await _authenticationService.ChangePasswordAsync(GetCurrentUserId(), request);
        return Ok(ApiResponse<object>.Create(null, "Password changed successfully."));
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("Authenticated user identifier is invalid.");
        }

        return parsedUserId;
    }
}
