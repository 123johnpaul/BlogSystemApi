using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Repositories;

public interface IOtpRepository
{
    Task<Otp?> GetValidOtpAsync(
        Guid userId,
        string code);

    Task AddAsync(Otp otp);

    Task UpdateAsync(Otp otp);

    Task InvalidateUserOtpsAsync(
        Guid userId,
        OtpPurpose purpose);

    Task SaveChangesAsync();
}