namespace Visus.AddressValidation.Integration.Google.Tests;

using System.Text.Json;
using AddressValidation.Http;
using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Services;

public sealed class GoogleAuthenticationServiceFacts(ConfigurationFixture fixture) : IClassFixture<ConfigurationFixture>
{
    private readonly ConfigurationFixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task GetAccessTokenAsync_Cached()
    {
        var response = new TokenResponse
        {
            AccessToken = Guid.NewGuid().ToString(),
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var distributedCacheOptions = Options.Create(new MemoryDistributedCacheOptions());
        var distributedCache = new MemoryDistributedCache(distributedCacheOptions);

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("https://oauth2.googleapis.com/token")
                              .WithFormData("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer")
                              .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = httpMessageHandlerMock.ToHttpClient();
        var client = new GoogleAuthenticationClient(_fixture.Configuration, httpClient);
        var service = new GoogleAuthenticationService(distributedCache, _fixture.Configuration, client);

        _ = await service.GetAccessTokenAsync();

        Assert.False(string.IsNullOrWhiteSpace(service.CacheKey));
        var accessToken = await distributedCache.GetStringAsync(service.CacheKey);

        Assert.Equal(response.AccessToken, accessToken);
        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetAccessTokenAsync_Default()
    {
        var response = new TokenResponse
        {
            AccessToken = Guid.NewGuid().ToString(),
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var distributedCacheMock = Substitute.For<IDistributedCache>();

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("https://oauth2.googleapis.com/token")
                              .WithFormData("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer")
                              .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = httpMessageHandlerMock.ToHttpClient();
        var client = new GoogleAuthenticationClient(_fixture.Configuration, httpClient);
        var service = new GoogleAuthenticationService(distributedCacheMock, _fixture.Configuration, client);

        var result = await service.GetAccessTokenAsync();

        Assert.Equal(response.AccessToken, result);

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }
}
