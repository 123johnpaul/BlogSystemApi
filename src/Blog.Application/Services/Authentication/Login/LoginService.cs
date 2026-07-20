using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Security;
using Blog.Domain.Entities;

namespace Blog.Application.Services.Authentication.Login;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenExpirationProvider _refreshTokenExpirationProvider;

    public LoginService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ITokenGenerator tokenGenerator,
        IRefreshTokenExpirationProvider refreshTokenExpirationProvider)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _tokenGenerator = tokenGenerator;
        _refreshTokenExpirationProvider = refreshTokenExpirationProvider;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        if (!user.IsVerified)
        {
            throw new InvalidOperationException("Please verify your email before logging in.");
        }

        var passwordMatches = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!passwordMatches)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _tokenGenerator.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(
                _refreshTokenExpirationProvider.GetRefreshTokenExpirationDays()),
            IsRevoked = false,
            RevokedAt = null,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };
    }
}
