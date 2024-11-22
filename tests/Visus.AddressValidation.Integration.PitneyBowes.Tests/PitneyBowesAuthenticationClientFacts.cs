namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using System.Net;
using System.Text.Json;
using AddressValidation.Http;
using Http;
using RichardSzalay.MockHttp;

public sealed class PitneyBowesAuthenticationClientFacts(ConfigurationFixture fixture) : IClassFixture<ConfigurationFixture>
{
	private readonly ConfigurationFixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

	[Fact]
	public async Task RequestClientCredentialsTokenAsync_Failed()
	{
		var httpMessageHandlerMock = new MockHttpMessageHandler();
		httpMessageHandlerMock.Expect("/oauth/token")
							  .WithFormData("grant_type", "client_credentials")
							  .Respond(HttpStatusCode.Unauthorized);

		var httpClient = httpMessageHandlerMock.ToHttpClient();

		var client = new PitneyBowesAuthenticationClient(_fixture.Configuration, httpClient);

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
		httpMessageHandlerMock.Expect("/oauth/token")
							  .WithFormData("grant_type", "client_credentials")
							  .Respond("application/json", JsonSerializer.Serialize(response));

		var httpClient = httpMessageHandlerMock.ToHttpClient();

		var client = new PitneyBowesAuthenticationClient(_fixture.Configuration, httpClient);

		var result = await client.RequestClientCredentialsTokenAsync();

		Assert.NotNull(result);
		Assert.Equal(response.AccessToken, result.AccessToken);
		Assert.Equal(response.ExpiresIn, result.ExpiresIn);

		httpMessageHandlerMock.VerifyNoOutstandingExpectation();
	}
}
