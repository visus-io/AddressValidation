namespace Visus.AddressValidation.Integration.FedEx.Tests;

using System.Text.Json;
using AddressValidation.Abstractions;
using AddressValidation.Models;
using AddressValidation.Services;
using AwesomeAssertions.Execution;
using Configuration;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

internal sealed class BatchAddressValidationServiceTests : IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IAddressValidationService<FedExAddressValidationRequest> _singularSut;

    private readonly IBatchAddressValidationService<FedExAddressValidationRequest> _sut;

    private readonly WireMockServer _wireMockServer;

    public BatchAddressValidationServiceTests()
    {
        _wireMockServer = WireMockServer.Start();

        ServiceCollection services = [];

        services.AddLogging()
                .AddHybridCache();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                                             .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                                              {
                                                  [$"{FedExServiceOptions.SectionName}:ClientId"] = "test-client-id",
                                                  [$"{FedExServiceOptions.SectionName}:ClientSecret"] = "test-client-secret",
                                                  [$"{FedExServiceOptions.SectionName}:AccountNumber"] = "test-account",
                                                  [$"{FedExServiceOptions.SectionName}:ClientEnvironment"] = nameof(ClientEnvironment.SANDBOX),
                                              })
                                             .Build());

        services.AddFedExAddressValidation();

        services.PostConfigure<FedExServiceOptions>(o => o.EndpointUriOverride = new Uri(_wireMockServer.Url!));

        _serviceProvider = services.BuildServiceProvider();
        _sut = _serviceProvider.GetRequiredService<IBatchAddressValidationService<FedExAddressValidationRequest>>();
        _singularSut = _serviceProvider.GetRequiredService<IAddressValidationService<FedExAddressValidationRequest>>();
    }

    [Test]
    public void AddFedExAddressValidation_RegistersBothSingularAndBatchServices()
    {
        using ( new AssertionScope() )
        {
            _singularSut.Should().NotBeNull();
            _sut.Should().NotBeNull();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
        _wireMockServer.Dispose();
    }

    [Test]
    public async Task ValidateManyAsync_WhenAllAddressesResolveCleanly_ReturnsResponsesInOriginalOrder(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubBatchResolve();

        IReadOnlyList<FedExAddressValidationRequest> requests = ThreeRequests();

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken)
                                                                       .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            results.Should().HaveCount(3);
            results[0]!.CityOrTown.Should().Be("IRVING");
            results[2]!.CityOrTown.Should().Be("NEW YORK");
        }
    }

    [Test]
    public async Task ValidateManyAsync_WhenApiReturnsErrorResponse_ReturnsSameErrorForEveryValidRequest(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubApiErrorResponse();

        IReadOnlyList<FedExAddressValidationRequest> requests = ThreeRequests();

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken)
                                                                       .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            results.Should().AllSatisfy(r => r!.Errors.Should().ContainSingle()
                                               .Which.Should().Be("STANDARDIZED.ADDRESS.NOTFOUND: Standardized address is not found."));
        }
    }

    [Test]
    public async Task ValidateManyAsync_WhenBatchExceeds100Addresses_ThrowsArgumentException(CancellationToken cancellationToken)
    {
        List<FedExAddressValidationRequest> requests = [.. Enumerable.Range(0, 101).Select(_ => ValidRequest()),];

        Func<Task> act = () => _sut.ValidateManyAsync(requests, cancellationToken);

        await act.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
    }

    [Test]
    public async Task ValidateManyAsync_WhenOneAddressHasInvalidSuiteNumber_OnlyThatIndexIsEmpty(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubBatchResolve();

        IReadOnlyList<FedExAddressValidationRequest> requests = ThreeRequests();

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken)
                                                                       .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            results[1].Should().BeOfType<EmptyAddressValidationResponse>();
            results[0].Should().NotBeOfType<EmptyAddressValidationResponse>();
            results[2].Should().NotBeOfType<EmptyAddressValidationResponse>();
        }
    }

    [Test]
    public async Task ValidateManyAsync_WhenRequestBeforeItFailsLocalValidation_ErrorMessageReferencesOriginalIndex(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubTwoItemBatchResolveWithSecondInvalidSuite();

        FedExAddressValidationRequest invalid = new()
        {
            Country = CountryCode.US,
        };

        List<FedExAddressValidationRequest> requests = [invalid, ValidRequest(), ValidRequest(),];

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken)
                                                                       .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            results[0].Should().BeOfType<EmptyAddressValidationResponse>();
            results[2].Should().BeOfType<EmptyAddressValidationResponse>();
            results[2]!.Errors.Should().Contain(e => e.Contains("[Row 2]", StringComparison.Ordinal));
        }
    }

    [Test]
    public async Task ValidateManyAsync_WhenSomeRequestsFailLocalValidation_OnlySendsValidOnesToApi(CancellationToken cancellationToken)
    {
        StubOAuthToken();

        FedExAddressValidationRequest invalid = new()
        {
            Country = CountryCode.US,
        };

        List<FedExAddressValidationRequest> requests = [ValidRequest(), invalid,];

        StubSingleAddressResolve();

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken)
                                                                       .ConfigureAwait(false);

        ILogEntry logEntry = _wireMockServer.LogEntries
                                            .Single(e => string.Equals(e.RequestMessage?.Path, "/address/v1/addresses/resolve", StringComparison.Ordinal));
        using JsonDocument sentRequest = JsonDocument.Parse(logEntry.RequestMessage?.Body ?? string.Empty);

        using ( new AssertionScope() )
        {
            results[1].Should().BeOfType<EmptyAddressValidationResponse>();
            results[1]!.Errors.Should().NotBeEmpty();
            sentRequest.RootElement.GetProperty("addressesToValidate").GetArrayLength().Should().Be(1);
        }
    }

    [Test]
    public async Task ValidateManyAsync_WhenTopLevelAlertIsWarning_AppliesWarningToEveryResolvedItem(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubBatchResolve();

        IReadOnlyList<FedExAddressValidationRequest> requests = ThreeRequests();

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken)
                                                                       .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            results[0]!.Warnings.Should().Contain(w => w.Contains("SHIP.RECIPIENT.POSTALCITY.MISMATCH", StringComparison.Ordinal));
            results[2]!.Warnings.Should().Contain(w => w.Contains("SHIP.RECIPIENT.POSTALCITY.MISMATCH", StringComparison.Ordinal));
        }
    }

    private static List<FedExAddressValidationRequest> ThreeRequests()
    {
        return [ValidRequest(), ValidRequest(), ValidRequest(),];
    }

    private static FedExAddressValidationRequest ValidRequest()
    {
        return new FedExAddressValidationRequest
        {
            AddressLines =
            {
                "7372 PARKRIDGE BLVD",
            },
            CityOrTown = "IRVING",
            StateOrProvince = "TX",
            PostalCode = "75063",
            Country = CountryCode.US,
        };
    }

    private void StubApiErrorResponse()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ApiErrorResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/address/v1/addresses/resolve").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(400)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubBatchResolve()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "BatchResolvedAddressesResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/address/v1/addresses/resolve").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubOAuthToken()
    {
        _wireMockServer.Given(Request.Create().WithPath("/oauth/token").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody("""{"access_token":"test-token","token_type":"bearer","expires_in":3600}"""));
    }

    private void StubSingleAddressResolve()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ResolvedAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/address/v1/addresses/resolve").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubTwoItemBatchResolveWithSecondInvalidSuite()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "BatchResolvedAddressesResponseTwoItemsSecondInvalidSuite.json"));

        _wireMockServer.Given(Request.Create().WithPath("/address/v1/addresses/resolve").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }
}
