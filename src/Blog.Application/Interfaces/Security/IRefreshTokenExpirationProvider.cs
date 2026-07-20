namespace Blog.Application.Interfaces.Security;

public interface IRefreshTokenExpirationProvider
{
    int GetRefreshTokenExpirationDays();
}
