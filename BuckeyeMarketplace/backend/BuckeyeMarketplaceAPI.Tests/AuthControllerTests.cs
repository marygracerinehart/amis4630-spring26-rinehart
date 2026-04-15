using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Integration tests for AuthController.
/// Tests registration, login, and validation.
/// NOTE: Because the test factory replaces JWT with a test auth scheme,
/// these tests validate the HTTP contract (status codes, response shapes)
/// but not actual JWT token generation.
/// </summary>
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ───────────────────────────────────────────────────────────────
    // POST /api/auth/register — Registration
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Register_ValidData_Returns200()
    {
        var payload = new
        {
            Email = $"testuser-{Guid.NewGuid():N}@example.com",
            Password = "TestPass123",
            ConfirmPassword = "TestPass123",
            FullName = "Test User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.TryGetProperty("token", out _), "Response should contain a token.");
        Assert.True(doc.RootElement.TryGetProperty("userId", out _), "Response should contain userId.");
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        var email = $"dup-{Guid.NewGuid():N}@example.com";

        var payload = new
        {
            Email = email,
            Password = "TestPass123",
            ConfirmPassword = "TestPass123",
            FullName = "First User"
        };

        // First registration should succeed
        var first = await _client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        // Second registration with same email should fail
        var second = await _client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_Returns400()
    {
        var payload = new
        {
            Email = $"weak-{Guid.NewGuid():N}@example.com",
            Password = "123", // too short, no uppercase
            ConfirmPassword = "123",
            FullName = "Weak User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ───────────────────────────────────────────────────────────────
    // POST /api/auth/login — Login
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ValidCredentials_Returns200()
    {
        var email = $"login-{Guid.NewGuid():N}@example.com";

        // Register first
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = "ValidPass1",
            ConfirmPassword = "ValidPass1",
            FullName = "Login User"
        });

        // Login
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = "ValidPass1"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.TryGetProperty("token", out _), "Login response should contain a token.");
        Assert.True(doc.RootElement.TryGetProperty("refreshToken", out _), "Login response should contain a refreshToken.");
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        var email = $"bad-pwd-{Guid.NewGuid():N}@example.com";

        // Register first
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = "ValidPass1",
            ConfirmPassword = "ValidPass1",
            FullName = "Bad Pwd User"
        });

        // Login with wrong password
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = "WrongPassword99"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonexistentUser_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "nobody-exists@example.com",
            Password = "DoesntMatter1"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
