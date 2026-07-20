using Blog.Application.Interfaces.Email;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Security;
using Blog.Application.Interfaces.Services;
using Blog.Application.Services.Authentication;
using Blog.Application.Services.Authentication.Login;
using Blog.Application.Services.Authentication.Registration;
using Blog.Application.Services.Authentication.Verification;
using Blog.Application.Services.Users;
using Blog.Application.Services.Posts;
using Blog.Application.Services.Comments;
using Blog.Infrastructure.Authentication;
using Blog.Infrastructure.Email;
using Blog.Infrastructure.Persistence;
using Blog.Infrastructure.Repositories;
using Blog.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Blog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT signing key is not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenExpirationProvider, RefreshTokenExpirationProvider>();

        // Infrastructure Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOtpService, OtpService>();

        // Application Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();

        return services;
    }
}
