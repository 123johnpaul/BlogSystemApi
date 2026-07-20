using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces.Email;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Security;
using Blog.Application.Interfaces.Services;
using Blog.Application.Services.Authentication.Login;
using Blog.Application.Services.Authentication.Registration;
using Blog.Application.Services.Authentication.Verification;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRegistrationService _registrationService;
    private readonly IVerificationService _verificationService;
    private readonly ILoginService _loginService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenExpirationProvider _refreshTokenExpirationProvider;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public AuthenticationService(
        IRegistrationService registrationService,
        IVerificationService verificationService,
        ILoginService loginService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ITokenGenerator tokenGenerator,
        IRefreshTokenExpirationProvider refreshTokenExpirationProvider,
        IOtpService otpService,
        IEmailService emailService)
    {
        _registrationService = registrationService;
        _verificationService = verificationService;
        _loginService = loginService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _tokenGenerator = tokenGenerator;
        _refreshTokenExpirationProvider = refreshTokenExpirationProvider;
        _otpService = otpService;
        _emailService = emailService;
    }

    public Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        return _registrationService.RegisterAsync(request);
    }

    public Task VerifyEmailAsync(VerifyEmailRequest request)
    {
        return _verificationService.VerifyEmailAsync(request);
    }

    public Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        return _loginService.LoginAsync(request);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingToken is null || existingToken.IsRevoked || existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Invalid or expired refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId);

        if (user is null || !user.IsVerified)
        {
            throw new InvalidOperationException("Invalid or expired refresh token.");
        }

        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;

        var replacementToken = CreateRefreshToken(user.Id);

        await _refreshTokenRepository.UpdateAsync(existingToken);
        await _refreshTokenRepository.AddAsync(replacementToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = replacementToken.Token,
            ExpiresAt = replacementToken.ExpiresAt
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        var tokens = await _refreshTokenRepository.GetUserTokensAsync(userId);

        foreach (var token in tokens.Where(token => !token.IsRevoked))
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token);
        }

        await _refreshTokenRepository.SaveChangesAsync();
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return;
        }

        var otp = await _otpService.GenerateOtpAsync(user, OtpPurpose.PasswordReset);

        await _emailService.SendEmailAsync(
            user.Email,
            "Reset your password",
            $"<h2>Password reset</h2><p>Your password reset code is:</p><h1>{otp.Code}</h1><p>This code expires in 10 minutes.</p>");
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new InvalidOperationException("Invalid or expired password reset code.");
        }

        var isValid = await _otpService.VerifyOtpAsync(user, request.Code, OtpPurpose.PasswordReset);

        if (!isValid)
        {
            throw new InvalidOperationException("Invalid or expired password reset code.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await RevokeUserTokensAsync(user.Id);
        await _userRepository.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await RevokeUserTokensAsync(user.Id);
        await _userRepository.SaveChangesAsync();
    }

    private RefreshToken CreateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = _tokenGenerator.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationProvider.GetRefreshTokenExpirationDays()),
            CreatedAt = DateTime.UtcNow
        };
    }

    private async Task RevokeUserTokensAsync(Guid userId)
    {
        var tokens = await _refreshTokenRepository.GetUserTokensAsync(userId);

        foreach (var token in tokens.Where(token => !token.IsRevoked))
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token);
        }

        await _refreshTokenRepository.SaveChangesAsync();
    }
}
