using NSubstitute;
using NSubstitute.Exceptions;

namespace Visus.AddressValidation.Integration.Ups.Tests;

using System.Text.Json;
using AddressValidation.Http;
using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using Services;

public sealed class UpsAuthenticationServiceFacts(ConfigurationFixture fixture) : IClassFixture<ConfigurationFixture>
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
		httpMessageHandlerMock.Expect("/security/v1/oauth/token")
							  .WithFormData("grant_type", "client_credentials")
							  .WithHeaders("x-merchant-id", _fixture.Configuration[Constants.AccountNumberConfigurationKey]!)
							  .Respond("application/json", JsonSerializer.Serialize(response));

		var httpClient = httpMessageHandlerMock.ToHttpClient();
		var client = new UpsAuthenticationClient(_fixture.Configuration, httpClient);
		var service = new UpsAuthenticationService(distributedCache, _fixture.Configuration, client);

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
		httpMessageHandlerMock.Expect("/security/v1/oauth/token")
							  .WithFormData("grant_type", "client_credentials")
							  .WithHeaders("x-merchant-id", _fixture.Configuration[Constants.AccountNumberConfigurationKey]!)
							  .Respond("application/json", JsonSerializer.Serialize(response));

		var httpClient = httpMessageHandlerMock.ToHttpClient();
		var client = new UpsAuthenticationClient(_fixture.Configuration, httpClient);
		var service = new UpsAuthenticationService(distributedCacheMock, _fixture.Configuration, client);

		var result = await service.GetAccessTokenAsync();

		Assert.Equal(response.AccessToken, result);

		httpMessageHandlerMock.VerifyNoOutstandingExpectation();
	}
}
