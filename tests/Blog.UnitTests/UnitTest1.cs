using Blog.Application.DTOs.Auth;
using Blog.Application.Validators;

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
