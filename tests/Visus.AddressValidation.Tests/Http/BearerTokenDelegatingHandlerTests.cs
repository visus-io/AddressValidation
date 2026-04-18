namespace Visus.AddressValidation.Tests.Http;

using System.Net;
using System.Security.Authentication;
using AddressValidation.Http;
using AwesomeAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Services;

internal sealed class BearerTokenDelegatingHandlerTests
{
    private static HybridCache CreateCache()
    {
        ServiceCollection services = new();
        services.AddHybridCache();
        return services.BuildServiceProvider().GetRequiredService<HybridCache>();
    }

    [Test]
    public void Constructor_NullAuthenticationService_ThrowsArgumentNullException()
    {
        Action act = () => _ = new BearerTokenDelegatingHandler<TestAuthenticationClient>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task SendAsync_WithoutToken_ThrowsInvalidCredentialException()
    {
        HybridCache cache = CreateCache();

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));
        TestAuthenticationService authService = new(client, cache);

        using BearerTokenDelegatingHandler<TestAuthenticationClient> handler = new(authService)
        {
            InnerHandler = new TestMessageHandler(),
        };

        using HttpClient httpClient = new(handler, false);

        Func<Task> act = () => httpClient.GetAsync(new Uri("https://example.com"));

        await act.Should().ThrowExactlyAsync<InvalidCredentialException>().ConfigureAwait(false);
    }

    [Test]
    public async Task SendAsync_WithToken_AddsBearerHeader()
    {
        HybridCache cache = CreateCache();

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(new TokenResponse
        {
            AccessToken = "my-token",
            ExpiresIn = 3600,
        }));
        TestAuthenticationService authService = new(client, cache);

        TestMessageHandler inner = new();
        using BearerTokenDelegatingHandler<TestAuthenticationClient> handler = new(authService)
        {
            InnerHandler = inner,
        };

        using HttpClient httpClient = new(handler, false);
        HttpResponseMessage response = await httpClient.GetAsync(new Uri("https://example.com")).ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        inner.LastRequest.Should().NotBeNull();
        inner.LastRequest!.Headers.Authorization.Should().NotBeNull();
        inner.LastRequest.Headers.Authorization!.Scheme.Should().Be("Bearer");
        inner.LastRequest.Headers.Authorization.Parameter.Should().Be("my-token");
    }

    private sealed class TestMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
