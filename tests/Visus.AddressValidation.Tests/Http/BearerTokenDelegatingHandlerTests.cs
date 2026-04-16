namespace Visus.AddressValidation.Tests.Http;

using System.Net;
using System.Security.Authentication;
using System.Text;
using AddressValidation.Http;
using AwesomeAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Services;

internal sealed class BearerTokenDelegatingHandlerTests
{
    [Test]
    public void Constructor_NullAuthenticationService_ThrowsArgumentNullException()
    {
        Action act = () => _ = new BearerTokenDelegatingHandler<TestAuthenticationClient>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task SendAsync_WithoutToken_ThrowsInvalidCredentialException()
    {
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache.GetAsync("test-key", Arg.Any<CancellationToken>())
             .Returns((byte[]?)null);

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
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache.GetAsync("test-key", Arg.Any<CancellationToken>())
             .Returns(Encoding.UTF8.GetBytes("my-token"));

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));
        TestAuthenticationService authService = new(client, cache);

        using BearerTokenDelegatingHandler<TestAuthenticationClient> handler = new(authService)
        {
            InnerHandler = new TestMessageHandler(),
        };

        using HttpClient httpClient = new(handler, false);
        HttpResponseMessage response = await httpClient.GetAsync(new Uri("https://example.com")).ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed class TestMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
