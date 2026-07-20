using System.Security.Cryptography;
using Blog.Application.Interfaces.Security;

namespace Blog.Infrastructure.Security;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}