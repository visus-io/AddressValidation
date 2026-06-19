namespace Visus.AddressValidation.Integration.Ups.Tests;

using System.Security.Authentication;
using AddressValidation.Abstractions;
using AddressValidation.Models;
using AddressValidation.Services;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Configuration;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

internal sealed class AddressValidationServiceTests : IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IAddressValidationService<UpsAddressValidationRequest> _sut;

    private readonly WireMockServer _wireMockServer;

    public AddressValidationServiceTests()
    {
        _wireMockServer = WireMockServer.Start();

        ServiceCollection services = [];

        services.AddLogging()
                .AddHybridCache();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                                             .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                                              {
                                                  [$"{UpsServiceOptions.SectionName}:ClientId"] = "test-client-id",
                                                  [$"{UpsServiceOptions.SectionName}:ClientSecret"] = "test-client-secret",
                                                  [$"{UpsServiceOptions.SectionName}:AccountNumber"] = "test-account-number",
                                                  [$"{UpsServiceOptions.SectionName}:ClientEnvironment"] = nameof(ClientEnvironment.SANDBOX),
                                              })
                                             .Build());

        services.AddUpsAddressValidation();

        services.PostConfigure<UpsServiceOptions>(o =>
        {
            o.EndpointUriOverride = new Uri(_wireMockServer.Url!);
        });

        _serviceProvider = services.BuildServiceProvider();
        _sut = _serviceProvider.GetRequiredService<IAddressValidationService<UpsAddressValidationRequest>>();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
        _wireMockServer.Dispose();
    }

    [Test]
    public async Task ValidateAsync_WhenAddressHasNoCandidates_ReturnsEmptyResponse(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubNoCandidatesResponse();

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "1 Nonexistent Blvd",
            },
            CityOrTown = "Nowhere",
            StateOrProvince = "CA",
            PostalCode = "90210",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Warnings.Should().BeEmpty();
            response.Suggestions.Should().BeEmpty();
            response.AddressLines.Should().BeEmpty();
        }
    }

    [Test]
    public async Task ValidateAsync_WhenAddressIsResolved_ReturnsExpectedResponse(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubAddressResolve();

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "CA",
            PostalCode = "91302-1985",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Warnings.Should().BeEmpty();
            response.Suggestions.Should().BeEmpty();
            response.AddressLines.Should().BeEquivalentTo("26601 Agoura Rd");
            response.CityOrTown.Should().Be("Calabasas");
            response.StateOrProvince.Should().Be("CA");
            response.PostalCode.Should().Be("91302-1985");
            response.Country.Should().Be(CountryCode.US);
            response.IsResidential.Should().BeFalse();
        }
    }

    [Test]
    public async Task ValidateAsync_WhenAddressValidationReturnsCandidates_ReturnsSuggestionsInResponse(
        CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubAmbiguousAddressResolve();

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "500 Market St",
            },
            CityOrTown = "San Francisco",
            StateOrProvince = "CA",
            PostalCode = "94105",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Suggestions.Should().ContainSingle()
                    .Which.AddressLines.Should().Contain("500 MARKET ST FL 2");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenApiReturnsErrorResponse_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubApiErrorResponse();

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "CA",
            PostalCode = "91302",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Be("111210: Missing or invalid street address");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenInDevelopmentModeWithNonUsCountry_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        IAddressValidationService<UpsAddressValidationRequest> sut = BuildServiceWithEnvironment(ClientEnvironment.DEVELOPMENT);

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Toronto",
            StateOrProvince = "ON",
            PostalCode = "M5V 2T6",
            Country = CountryCode.CA,
        };

        IAddressValidationResponse? response = await sut.ValidateAsync(request, cancellationToken)
                                                        .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Contain(nameof(request.Country));
        }
    }

    [Test]
    public async Task ValidateAsync_WhenInDevelopmentModeWithUnsupportedState_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        IAddressValidationService<UpsAddressValidationRequest> sut = BuildServiceWithEnvironment(ClientEnvironment.DEVELOPMENT);

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "TX",
            PostalCode = "91302",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await sut.ValidateAsync(request, cancellationToken)
                                                        .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Contain(nameof(request.StateOrProvince));
        }
    }

    [Test]
    public async Task ValidateAsync_WhenMaximumCandidateListSizeExceedsMaximum_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "CA",
            PostalCode = "91302",
            Country = CountryCode.US,
            MaximumCandidateListSize = 51,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Contain(nameof(request.MaximumCandidateListSize));
        }
    }

    [Test]
    public async Task ValidateAsync_WhenMaximumCandidateListSizeIsNegative_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "CA",
            PostalCode = "91302",
            Country = CountryCode.US,
            MaximumCandidateListSize = -1,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Contain(nameof(request.MaximumCandidateListSize));
        }
    }

    [Test]
    public async Task ValidateAsync_WhenOAuthReturnsEmptyToken_ThrowsInvalidCredentialException(
        CancellationToken cancellationToken)
    {
        _wireMockServer.Given(Request.Create().WithPath("/security/v1/oauth/token").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody("""{"access_token":"","token_type":"bearer","expires_in":3600}"""));

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "CA",
            PostalCode = "91302",
            Country = CountryCode.US,
        };

        Func<Task<IAddressValidationResponse?>> act = () => _sut.ValidateAsync(request, cancellationToken);
        await act.Should().ThrowAsync<InvalidCredentialException>().ConfigureAwait(false);
    }

    [Test]
    [Arguments(401)]
    [Arguments(403)]
    [Arguments(500)]
    public async Task ValidateAsync_WhenOAuthTokenRequestFails_ThrowsHttpRequestException(
        int statusCode, CancellationToken cancellationToken)
    {
        _wireMockServer.Given(Request.Create().WithPath("/security/v1/oauth/token").UsingPost())
                       .RespondWith(Response.Create().WithStatusCode(statusCode));

        UpsAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "26601 Agoura Rd",
            },
            CityOrTown = "Calabasas",
            StateOrProvince = "CA",
            PostalCode = "91302",
            Country = CountryCode.US,
        };

        Func<Task<IAddressValidationResponse?>> act = () => _sut.ValidateAsync(request, cancellationToken);
        await act.Should().ThrowAsync<HttpRequestException>().ConfigureAwait(false);
    }

    private IAddressValidationService<UpsAddressValidationRequest> BuildServiceWithEnvironment(ClientEnvironment environment)
    {
        ServiceCollection services = [];

        services.AddLogging()
                .AddHybridCache();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                                             .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                                              {
                                                  [$"{UpsServiceOptions.SectionName}:ClientId"] = "test-client-id",
                                                  [$"{UpsServiceOptions.SectionName}:ClientSecret"] = "test-client-secret",
                                                  [$"{UpsServiceOptions.SectionName}:AccountNumber"] = "test-account-number",
                                                  [$"{UpsServiceOptions.SectionName}:ClientEnvironment"] = environment.ToString(),
                                              })
                                             .Build());

        services.AddUpsAddressValidation();

        services.PostConfigure<UpsServiceOptions>(o =>
        {
            o.EndpointUriOverride = new Uri(_wireMockServer.Url!);
        });

        return services.BuildServiceProvider()
                       .GetRequiredService<IAddressValidationService<UpsAddressValidationRequest>>();
    }

    private void StubAddressResolve()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ResolvedAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/api/addressvalidation/v2/3").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubAmbiguousAddressResolve()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "AmbiguousAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/api/addressvalidation/v2/3").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubApiErrorResponse()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ApiErrorResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/api/addressvalidation/v2/3").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(400)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubNoCandidatesResponse()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "NoCandidatesResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/api/addressvalidation/v2/3").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubOAuthToken()
    {
        _wireMockServer.Given(Request.Create().WithPath("/security/v1/oauth/token").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody("""{"access_token":"test-token","token_type":"bearer","expires_in":3600}"""));
    }
}
