using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces.Email;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Security;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services.Authentication.Registration;

public class RegistrationService : IRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public RegistrationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
        _emailService = emailService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            Surname = request.Surname,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = UserRole.User,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var otp = await _otpService.GenerateOtpAsync(
            user,
            OtpPurpose.EmailVerification);

        var emailBody = $"""
            <h2>Email Verification</h2>

            <p>Hello {user.FirstName},</p>

            <p>Your verification code is:</p>

            <h1>{otp.Code}</h1>

            <p>This code expires in 10 minutes.</p>
            """;

        await _emailService.SendEmailAsync(
            user.Email,
            "Verify your email",
            emailBody);

        return new RegisterResponse
        {
            UserId = user.Id,
            Message = "Registration successful. Please verify your email."
        };
    }
}