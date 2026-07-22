using Blog.Application.DTOs.Auth;
using Blog.Application.Validators;
using Blog.Infrastructure.Security;

namespace Blog.UnitTests;

public class RegisterRequestValidatorTests
{
    [Fact]
    public void Rejects_a_password_that_does_not_meet_the_password_policy()
    {
        var request = new RegisterRequest
        {
            FirstName = "Jane",
            Surname = "Doe",
            Email = "jane@example.com",
            Password = "weakpass",
            ConfirmPassword = "weakpass"
        };

        var result = new RegisterRequestValidator().Validate(request);

        Assert.False(result.IsValid);
    }
}

public class SecurityComponentTests
{
    [Fact]
    public void Password_hasher_hashes_and_verifies_a_password()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("ValidPassword1!");

        Assert.NotEqual("ValidPassword1!", hash);
        Assert.True(hasher.VerifyPassword("ValidPassword1!", hash));
        Assert.False(hasher.VerifyPassword("WrongPassword1!", hash));
    }

    [Fact]
    public void Refresh_token_generator_creates_distinct_nonempty_tokens()
    {
        var generator = new TokenGenerator();

        var first = generator.GenerateRefreshToken();
        var second = generator.GenerateRefreshToken();

        Assert.NotEmpty(first);
        Assert.NotEqual(first, second);
    }
}
