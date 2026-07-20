using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ApplicationDbContext _context;

    public OtpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Otp?> GetValidOtpAsync(
        Guid userId,
        string code)
    {
        return await _context.Otps
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Code == code &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);
    }

    public async Task AddAsync(Otp otp)
    {
        await _context.Otps.AddAsync(otp);
    }

    public Task UpdateAsync(Otp otp)
    {
        _context.Otps.Update(otp);
        return Task.CompletedTask;
    }

    public async Task InvalidateUserOtpsAsync(
        Guid userId,
        OtpPurpose purpose)
    {
        var otps = await _context.Otps
            .Where(x =>
                x.UserId == userId &&
                x.Purpose == purpose &&
                !x.IsUsed)
            .ToListAsync();

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}