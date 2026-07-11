namespace Visus.AddressValidation.Tests.Services;

using System.Diagnostics;
using AddressValidation.Services;
using Http;
using Http.Clients;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

internal sealed class AbstractAuthenticationServiceTests : IAsyncDisposable
{
    private readonly IAuthenticationClient _authenticationClient;

    private readonly HybridCache _cache;

    private readonly DiagnosticsCapture _capture = new();

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
        _capture.Dispose();
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
    }

    [Test]
    [NotInParallel]
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
    [NotInParallel]
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
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenCacheHit_DoesNotRecordActivityOrHistogram(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        int activityCountBeforeCacheHit = _capture.Activities.Count(a => string.Equals(a.OperationName, "address_validation.token_fetch", StringComparison.Ordinal));
        int measurementCountBeforeCacheHit =
            _capture.Measurements.Count(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.duration", StringComparison.Ordinal));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        _capture.Activities.Count(a => string.Equals(a.OperationName, "address_validation.token_fetch", StringComparison.Ordinal)).Should().Be(activityCountBeforeCacheHit);
        _capture.Measurements.Count(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.duration", StringComparison.Ordinal))
                .Should()
                .Be(measurementCountBeforeCacheHit);
    }

    [Test]
    [NotInParallel]
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
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenCacheMiss_RecordsTokenFetchActivityAndHistogramWithSuccessResult(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        Activity fetchActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.token_fetch", StringComparison.Ordinal)).Subject;
        fetchActivity.GetTagItem("address_validation.client_type").Should().Be(nameof(IAuthenticationClient));
        fetchActivity.GetTagItem("address_validation.result").Should().Be("success");
        fetchActivity.Status.Should().Be(ActivityStatusCode.Unset);

        DiagnosticsCapture.Measurement measurement = _capture.Measurements
                                                             .Should()
                                                             .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.duration", StringComparison.Ordinal))
                                                             .Subject;
        measurement.Value.Should().BeGreaterThanOrEqualTo(0);
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.client_type", nameof(IAuthenticationClient)));
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.result", "success"));
    }

    [Test]
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenCacheMiss_RecordsCacheResultCounterTaggedMiss(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        DiagnosticsCapture.Measurement measurement = _capture.Measurements
                                                             .Should()
                                                             .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.cache_result", StringComparison.Ordinal))
                                                             .Subject;
        measurement.Value.Should().Be(1);
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.client_type", nameof(IAuthenticationClient)));
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.cache_result", "miss"));
    }

    [Test]
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenCacheHit_RecordsCacheResultCounterTaggedHit(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        DiagnosticsCapture.Measurement measurement = _capture.Measurements
                                                             .Should()
                                                             .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.cache_result", StringComparison.Ordinal)
                                                                               && m.Tags.Contains(new KeyValuePair<string, object?>("address_validation.cache_result", "hit")))
                                                             .Subject;
        measurement.Value.Should().Be(1);
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.client_type", nameof(IAuthenticationClient)));
    }

    [Test]
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenCalledTwiceWithCacheMissThenHit_RecordsExactlyOneMissAndOneHit(CancellationToken cancellationToken)
    {
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(validResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        DiagnosticsCapture.Measurement[] cacheResultMeasurements =
        [
            .. _capture.Measurements
                       .Where(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.cache_result", StringComparison.Ordinal)),
        ];

        cacheResultMeasurements.Count(m => m.Tags.Contains(new KeyValuePair<string, object?>("address_validation.cache_result", "miss"))).Should().Be(1);
        cacheResultMeasurements.Count(m => m.Tags.Contains(new KeyValuePair<string, object?>("address_validation.cache_result", "hit"))).Should().Be(1);
    }

    [Test]
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenClientReturnsEmptyAccessToken_RecordsResultTagEmptyToken(CancellationToken cancellationToken)
    {
        TokenResponse emptyTokenResponse = new(string.Empty, null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<TokenResponse?>(emptyTokenResponse));

        await _sut.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        Activity fetchActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.token_fetch", StringComparison.Ordinal)).Subject;
        fetchActivity.GetTagItem("address_validation.result").Should().Be("empty_token");
    }

    [Test]
    [NotInParallel]
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
    [NotInParallel]
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
    [NotInParallel]
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
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenClientThrows_RecordsResultTagErrorAndPropagatesException(CancellationToken cancellationToken)
    {
        InvalidOperationException thrown = new("client failed");
        _authenticationClient.RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>()).Returns<Task<TokenResponse?>>(_ => throw thrown);

        Func<Task> act = () => _sut.GetAccessTokenAsync(cancellationToken);

        await act.Should().ThrowExactlyAsync<InvalidOperationException>().ConfigureAwait(false);

        Activity fetchActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.token_fetch", StringComparison.Ordinal)).Subject;
        fetchActivity.GetTagItem("address_validation.result").Should().Be("error");
        fetchActivity.Status.Should().Be(ActivityStatusCode.Error);
    }

    [Test]
    [NotInParallel]
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

        Task<string?>[] tasks =
        [
            .. Enumerable
              .Range(0, concurrentRequests)
              .Select(_ => _sut.GetAccessTokenAsync(cancellationToken)),
        ];

        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        gate.Release();

        string?[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

        results.Should().AllSatisfy(r => r.Should().Be("test-token"));
        callCount.Should().Be(1);
    }

    [Test]
    [NotInParallel]
    public async Task GetAccessTokenAsync_WhenConcurrentCacheMisses_RecordsExactlyOneTokenFetchActivity(CancellationToken cancellationToken)
    {
        using SemaphoreSlim gate = new(0, 1);
        TokenResponse validResponse = new("test-token", null, null, null, 3600, null, "Bearer", null);

        _authenticationClient
           .RequestClientCredentialsTokenAsync(Arg.Any<CancellationToken>())
           .Returns(async _ =>
            {
                await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
                return (TokenResponse?)validResponse;
            });

        const int concurrentRequests = 10;

        Task<string?>[] tasks =
        [
            .. Enumerable
              .Range(0, concurrentRequests)
              .Select(_ => _sut.GetAccessTokenAsync(cancellationToken)),
        ];

        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        gate.Release();

        await Task.WhenAll(tasks).ConfigureAwait(false);

        _capture.Activities.Count(a => string.Equals(a.OperationName, "address_validation.token_fetch", StringComparison.Ordinal)).Should().Be(1);
        _capture.Measurements.Count(m => string.Equals(m.InstrumentName, "visus.address_validation.token_fetch.duration", StringComparison.Ordinal)).Should().Be(1);
    }

    [Test]
    [NotInParallel]
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
