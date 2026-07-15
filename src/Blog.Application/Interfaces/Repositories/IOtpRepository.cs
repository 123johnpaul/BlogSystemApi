using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories;

public interface IOtpRepository
{
    Task<Otp?> GetValidOtpAsync(Guid userId, string code);

    Task AddAsync(Otp otp);

    Task UpdateAsync(Otp otp);

    Task SaveChangesAsync();
}