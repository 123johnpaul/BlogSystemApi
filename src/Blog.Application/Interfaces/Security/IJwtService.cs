using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Security;

public interface IJwtService
{
    string GenerateAccessToken(User user);
}