namespace Visus.AddressValidation.Tests.Serialization.Json;

using System.Text.Json;
using AddressValidation.Http;
using AwesomeAssertions;

internal sealed class TokenResponseConverterTests
{
    [Test]
    public void Read_ExpiresInAsInvalidString_DefaultsToZero()
    {
        const string json = """{"expires_in": "not_a_number"}""";

        TokenResponse? result = JsonSerializer.Deserialize<TokenResponse>(json);

        result!.ExpiresIn.Should().Be(0);
    }

    [Test]
    public void Read_ExpiresInAsString_ParsesCorrectly()
    {
        const string json = """{"expires_in": "7200"}""";

        TokenResponse? result = JsonSerializer.Deserialize<TokenResponse>(json);

        result!.ExpiresIn.Should().Be(7200);
    }

    [Test]
    public void Read_PascalCaseJson_DeserializesCorrectly()
    {
        const string json = """
                            {
                                "AccessToken": "abc123",
                                "ExpiresIn": 1800,
                                "TokenType": "Bearer"
                            }
                            """;

        TokenResponse? result = JsonSerializer.Deserialize<TokenResponse>(json);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("abc123");
        result.ExpiresIn.Should().Be(1800);
        result.TokenType.Should().Be("Bearer");
    }

    [Test]
    public void Read_SnakeCaseJson_DeserializesCorrectly()
    {
        const string json = """
                            {
                                "access_token": "abc123",
                                "error_description": "none",
                                "expires_in": 3600,
                                "identity_token": "id123",
                                "issued_token_type": "bearer",
                                "refresh_token": "ref456",
                                "token_type": "Bearer"
                            }
                            """;

        TokenResponse? result = JsonSerializer.Deserialize<TokenResponse>(json);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("abc123");
        result.ErrorDescription.Should().Be("none");
        result.ExpiresIn.Should().Be(3600);
        result.IdentityToken.Should().Be("id123");
        result.IssuedTokenType.Should().Be("bearer");
        result.RefreshToken.Should().Be("ref456");
        result.TokenType.Should().Be("Bearer");
    }

    [Test]
    public void Write_NullValue_WritesNull()
    {
        string json = JsonSerializer.Serialize<TokenResponse?>(null);

        json.Should().Be("null");
    }

    [Test]
    public void Write_SerializesToSnakeCase()
    {
        TokenResponse response = new()
        {
            AccessToken = "token",
            ExpiresIn = 3600,
            TokenType = "Bearer",
        };

        string json = JsonSerializer.Serialize(response);
        JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        root.GetProperty("access_token").GetString().Should().Be("token");
        root.GetProperty("expires_in").GetInt32().Should().Be(3600);
        root.GetProperty("token_type").GetString().Should().Be("Bearer");
    }
}
