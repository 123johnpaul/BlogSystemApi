using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Blog.IntegrationTests;

public class HealthEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_success()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public class ApiFactory : WebApplicationFactory<Program>
{
    public ApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Key", "test-signing-key-with-at-least-thirty-two-characters");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "BlogSystemTests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "BlogSystemTests");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationMinutes", "15");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationDays", "7");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Host=localhost;Database=blog_system_tests;Username=postgres;Password=postgres");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
