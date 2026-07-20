using System.Security.Cryptography;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Infrastructure.Authentication;

public class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;

    public OtpService(IOtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<Otp> GenerateOtpAsync(User user, OtpPurpose purpose)
    {
        await _otpRepository.InvalidateUserOtpsAsync(user.Id, purpose);

        var code = RandomNumberGenerator.GetInt32(100000, 1_000_000).ToString();

        var otp = new Otp
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            Purpose = purpose,
            IsUsed = false,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        await _otpRepository.AddAsync(otp);

        await _otpRepository.SaveChangesAsync();

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(
        User user,
        string code,
        OtpPurpose purpose)
    {
        var otp = await _otpRepository.GetValidOtpAsync(
            user.Id,
            code);

        if (otp is null)
            return false;

        if (otp.Purpose != purpose)
            return false;

        otp.IsUsed = true;

        await _otpRepository.UpdateAsync(otp);

        await _otpRepository.SaveChangesAsync();

        return true;
    }
}
