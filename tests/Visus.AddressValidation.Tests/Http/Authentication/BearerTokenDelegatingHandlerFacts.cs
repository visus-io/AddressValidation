using NSubstitute;

namespace Visus.AddressValidation.Tests.Http.Authentication;

using Abstractions;
using AddressValidation.Http;
using Microsoft.Extensions.Caching.Distributed;
using Services;

public sealed class BearerTokenDelegatingHandlerFacts : DelegatingHandlerFacts
{
	[Fact]
	public async Task SendAsync_Success()
	{
		const string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

		var distributedCacheMock = Substitute.For<IDistributedCache>();
		var authenticationClient = new TestAuthenticationClient();
		var authenticationService = new TestAuthenticationService(authenticationClient, distributedCacheMock);

		var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
		var handler = new BearerTokenDelegatingHandler<TestAuthenticationClient>(authenticationService)
		{
			InnerHandler = new TestHandler()
		};

		var invoker = new HttpMessageInvoker(handler);
		_ = await invoker.SendAsync(httpRequestMessage, default);

		Assert.NotNull(httpRequestMessage.Headers.Authorization);
		Assert.Contains(accessToken, httpRequestMessage.Headers.Authorization.ToString());
	}

	private class TestAuthenticationClient : IAuthenticationClient
	{
		public ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
		{
			return new ValueTask<TokenResponse?>(new TokenResponse
			{
				AccessToken = "\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
			});
		}
	}

	private class TestAuthenticationService(TestAuthenticationClient authenticationClient, IDistributedCache cache)
		: AbstractAuthenticationService<TestAuthenticationClient>(authenticationClient, cache)
	{
		protected override string? GenerateCacheKey()
		{
			return "TEST";
		}
	}
}
