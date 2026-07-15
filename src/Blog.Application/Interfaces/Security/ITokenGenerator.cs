namespace Blog.Application.Interfaces.Security;

public interface ITokenGenerator
{
    string GenerateRefreshToken();
}