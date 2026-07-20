using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId);

    Task AddAsync(RefreshToken refreshToken);

    Task UpdateAsync(RefreshToken refreshToken);

    Task DeleteAsync(RefreshToken refreshToken);

    Task SaveChangesAsync();
}