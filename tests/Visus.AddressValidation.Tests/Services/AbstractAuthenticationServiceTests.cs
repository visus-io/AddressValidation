namespace Visus.AddressValidation.Tests.Services;

using AddressValidation.Services;
using AwesomeAssertions;
using Http;
using Http.Clients;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

internal sealed class AbstractAuthenticationServiceTests : IAsyncDisposable
{
    private readonly IAuthenticationClient _authenticationClient;

    private readonly HybridCache _cache;

    private readonly ServiceProvider _serviceProvider;

    private readonly TestAuthenticationService _sut;

    public AbstractAuthenticationServiceTests()
    {
        _authenticationClient = Substitute.For<IAuthenticationClient>();

        ServiceCollection services = [];
        services.AddHybridCache();
        _serviceProvider = services.BuildServiceProvider();

        _cache = _serviceProvider.GetRequiredService<HybridCache>();
        _sut = new TestAuthenticationService(_authenticationClient, _cache);
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenCachedTokenHasNullAccessToken_ReturnsNull(CancellationToken cancellationToken)
    {
        TokenResponse nullAccessTokenResponse = new(null, null, null, null, 3600, null, "Bearer", null);

        await _cache.SetAsync("vs-ave-auth:test:key", nullAccessTokenResponse, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        string? result = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        result.Should().BeNull();
        await _authenticationClient.DidNotReceive()
                                   .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
                                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenCacheHit_DoesNotCallClientAgain(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        string? result = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        result.Should().Be("test-token");
        await _authenticationClient.Received(1)
                                   .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
                                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenCacheMiss_CallsClientOnceAndReturnsToken(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        string? result = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        result.Should().Be("test-token");
        await _authenticationClient.Received(1)
                                   .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
                                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenClientReturnsEmptyAccessToken_ReturnsNullAndDoesNotCacheResult(CancellationToken cancellationToken)
    {
        TokenResponse emptyTokenResponse = new(string.Empty, null, null, null, 3600, null, "Bearer", null);
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(
                Task.FromResult<TokenResponse?>(emptyTokenResponse),
                Task.FromResult<TokenResponse?>(validResponse));

        string? firstResult = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        string? secondResult = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        firstResult.Should().BeNull();
        secondResult.Should().Be("test-token");
        await _authenticationClient.Received(2)
                                   .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
                                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenClientReturnsNullTokenResponse_ReturnsNullAndDoesNotCacheResult(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(
                Task.FromResult<TokenResponse?>(null),
                Task.FromResult<TokenResponse?>(validResponse));

        string? firstResult = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        string? secondResult = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        firstResult.Should().BeNull();
        secondResult.Should().Be("test-token");
        await _authenticationClient.Received(2)
                                   .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
                                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenClientReturnsWhitespaceAccessToken_ReturnsNullAndDoesNotCacheResult(CancellationToken cancellationToken)
    {
        TokenResponse whitespaceTokenResponse = new("   ", null, null, null, 3600, null, "Bearer", null);
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(
                Task.FromResult<TokenResponse?>(whitespaceTokenResponse),
                Task.FromResult<TokenResponse?>(validResponse));

        string? firstResult = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        string? secondResult = await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        firstResult.Should().BeNull();
        secondResult.Should().Be("test-token");
        await _authenticationClient.Received(2)
                                   .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
                                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenConcurrentCacheMisses_CallsClientOnlyOnce(CancellationToken cancellationToken)
    {
        using SemaphoreSlim gate = new(0, 1);
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);
        int callCount = 0;

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(async _ =>
            {
                Interlocked.Increment(ref callCount);
                await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
                return (TokenResponse?)validResponse;
            });

        const int concurrentRequests = 10;

        Task<string?>[] tasks = Enumerable
                               .Range(0, concurrentRequests)
                               .Select(_ => _sut.GetAccessTokenAsync(cancellationToken))
                               .ToArray();

        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        gate.Release();

        string?[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

        results.Should().AllSatisfy(r => r.Should().Be("test-token"));
        callCount.Should().Be(1);
    }

    [Test]
    public async Task GetAccessTokenAsync_WhenGenerateCacheKeyReturnsInvalidKey_ThrowsInvalidOperationException(CancellationToken cancellationToken)
    {
        InvalidCacheKeyAuthenticationService sut = new(_authenticationClient, _cache);

        Func<Task> act = () => sut.GetAccessTokenAsync(cancellationToken);

        await act.Should().ThrowExactlyAsync<InvalidOperationException>().ConfigureAwait(false);
    }

    private sealed class InvalidCacheKeyAuthenticationService(IAuthenticationClient client, HybridCache cache)
        : AbstractAuthenticationService<IAuthenticationClient>(client, cache)
    {
        protected override string GenerateCacheKey()
        {
            return "vs-ave-auth:test key with spaces!";
        }
    }

    private sealed class TestAuthenticationService(IAuthenticationClient client, HybridCache cache)
        : AbstractAuthenticationService<IAuthenticationClient>(client, cache)
    {
        protected override string GenerateCacheKey()
        {
            return "vs-ave-auth:test:key";
        }
    }
}
