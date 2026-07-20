using Blog.Application.Interfaces.Security;
using Microsoft.Extensions.Configuration;

namespace Blog.Infrastructure.Security;

public class RefreshTokenExpirationProvider : IRefreshTokenExpirationProvider
{
    private readonly IConfiguration _configuration;

    public RefreshTokenExpirationProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int GetRefreshTokenExpirationDays()
    {
        var configuredDays = _configuration.GetValue<int?>("Jwt:RefreshTokenExpirationDays");

        return configuredDays is > 0 ? configuredDays.Value : 7;
    }
}
