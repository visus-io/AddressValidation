namespace Visus.AddressValidation.Integration.Ups.Tests;

using System.Net;
using System.Text.Json;
using AddressValidation.Http;
using Http;
using Microsoft.Extensions.Configuration;
using RichardSzalay.MockHttp;

public sealed class UpsAuthenticationClientFacts(ConfigurationFixture fixture) : IClassFixture<ConfigurationFixture>
{
    private readonly ConfigurationFixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task RequestClientCredentialsTokenAsync_Failed()
    {
        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/security/v1/oauth/token")
                              .WithFormData("grant_type", "client_credentials")
                              .WithHeaders("x-merchant-id", _fixture.Configuration[Constants.AccountNumberConfigurationKey]!)
                              .Respond(HttpStatusCode.Unauthorized);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new UpsAuthenticationClient(_fixture.Configuration, httpClient);

        var result = await client.RequestClientCredentialsTokenAsync();

        Assert.Null(result);

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task RequestClientCredentialsTokenAsync_Success()
    {
        var response = new TokenResponse
        {
            AccessToken = Guid.NewGuid().ToString(),
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/security/v1/oauth/token")
                              .WithFormData("grant_type", "client_credentials")
                              .WithHeaders("x-merchant-id", _fixture.Configuration[Constants.AccountNumberConfigurationKey]!)
                              .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new UpsAuthenticationClient(_fixture.Configuration, httpClient);

        var result = await client.RequestClientCredentialsTokenAsync();

        Assert.NotNull(result);
        Assert.Equal(response.AccessToken, result.AccessToken);
        Assert.Equal(response.ExpiresIn, result.ExpiresIn);

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task RequestClientCredentialsTokenAsync_ThrowsInvalidOperationException()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var httpMessageHandlerMock = new MockHttpMessageHandler();
        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new UpsAuthenticationClient(configuration, httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.RequestClientCredentialsTokenAsync().ConfigureAwait(false));
    }
}
