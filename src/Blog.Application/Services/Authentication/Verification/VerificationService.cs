using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;

namespace Blog.Application.Services.Authentication.Verification;

public class VerificationService : IVerificationService
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;

    public VerificationService(
        IUserRepository userRepository,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _otpService = otpService;
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (user.IsVerified)
        {
            throw new InvalidOperationException("Email has already been verified.");
        }

        var isValid = await _otpService.VerifyOtpAsync(
            user,
            request.Code,
            Blog.Domain.Enums.OtpPurpose.EmailVerification);

        if (!isValid)
        {
            throw new InvalidOperationException("Invalid or expired verification code.");
        }

        user.IsVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}