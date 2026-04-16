namespace Visus.AddressValidation.Tests.Services;

using System.Text;
using AddressValidation.Http;
using AddressValidation.Services;
using AutoFixture;
using AwesomeAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

#pragma warning disable MA0048
internal sealed class TestAuthenticationClient : IAuthenticationClient
#pragma warning restore MA0048
{
    private readonly Func<CancellationToken, Task<TokenResponse?>> _func;

    public TestAuthenticationClient(Func<CancellationToken, Task<TokenResponse?>> func)
    {
        _func = func;
    }

    public Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        return _func(cancellationToken);
    }
}

#pragma warning disable MA0048
internal sealed class TestAuthenticationService : AbstractAuthenticationService<TestAuthenticationClient>
#pragma warning restore MA0048
{
    private readonly string? _cacheKey;

    public TestAuthenticationService(TestAuthenticationClient client, IDistributedCache cache, string? cacheKey = "test-key")
        : base(client, cache)
    {
        _cacheKey = cacheKey;
    }

    protected override string? GenerateCacheKey()
    {
        return _cacheKey;
    }
}

internal sealed class AbstractAuthenticationServiceTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void CacheKey_LazyInitialized_ReturnsGeneratedKey()
    {
        string cacheKey = _fixture.Create<string>();
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));
        TestAuthenticationService service = new(client, cache, cacheKey);

        service.CacheKey.Should().Be(cacheKey);
    }

    [Test]
    public void Constructor_NullCache_ThrowsArgumentNullException()
    {
        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));

        Action act = () => _ = new TestAuthenticationService(client, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_NullClient_ThrowsArgumentNullException()
    {
        IDistributedCache cache = Substitute.For<IDistributedCache>();

        Action act = () => _ = new TestAuthenticationService(null!, cache);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task GetAccessTokenAsync_CachedToken_ReturnsCachedValue()
    {
        string cacheKey = _fixture.Create<string>();
        string cachedToken = _fixture.Create<string>();
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache.GetAsync(cacheKey, Arg.Any<CancellationToken>())
             .Returns(Encoding.UTF8.GetBytes(cachedToken));

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));
        TestAuthenticationService service = new(client, cache, cacheKey);

        string? result = await service.GetAccessTokenAsync().ConfigureAwait(false);

        result.Should().Be(cachedToken);
    }

    [Test]
    public async Task GetAccessTokenAsync_CacheMiss_RequestsAndCachesToken()
    {
        string cacheKey = _fixture.Create<string>();
        string accessToken = _fixture.Create<string>();
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache.GetAsync(cacheKey, Arg.Any<CancellationToken>())
             .Returns((byte[]?)null);

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(new TokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = 3600,
        }));

        TestAuthenticationService service = new(client, cache, cacheKey);

        string? result = await service.GetAccessTokenAsync().ConfigureAwait(false);

        result.Should().Be(accessToken);
        await cache.Received(1)
                   .SetAsync(cacheKey, Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>())
                   .ConfigureAwait(false);
    }

    [Test]
    public async Task GetAccessTokenAsync_ClientReturnsNull_ReturnsNull()
    {
        string cacheKey = _fixture.Create<string>();
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache.GetAsync(cacheKey, Arg.Any<CancellationToken>())
             .Returns((byte[]?)null);

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));
        TestAuthenticationService service = new(client, cache, cacheKey);

        string? result = await service.GetAccessTokenAsync().ConfigureAwait(false);

        result.Should().BeNull();
    }

    [Test]
    public async Task GetAccessTokenAsync_EmptyAccessToken_ReturnsNull()
    {
        string cacheKey = _fixture.Create<string>();
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache.GetAsync(cacheKey, Arg.Any<CancellationToken>())
             .Returns((byte[]?)null);

        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(new TokenResponse
        {
            AccessToken = "",
            ExpiresIn = 3600,
        }));

        TestAuthenticationService service = new(client, cache, cacheKey);

        string? result = await service.GetAccessTokenAsync().ConfigureAwait(false);

        result.Should().BeNull();
    }

    [Test]
    public async Task GetAccessTokenAsync_NullCacheKey_ReturnsNull()
    {
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        TestAuthenticationClient client = new(_ => Task.FromResult<TokenResponse?>(null));
        TestAuthenticationService service = new(client, cache, null);

        string? result = await service.GetAccessTokenAsync().ConfigureAwait(false);

        result.Should().BeNull();
    }
}
