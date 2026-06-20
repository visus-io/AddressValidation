namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using System.Security.Authentication;
using AddressValidation.Abstractions;
using AddressValidation.Services;
using AwesomeAssertions.Execution;
using Configuration;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model;
using Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

internal sealed class AddressValidationServiceTests : IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IAddressValidationService<PitneyBowesAddressValidationRequest> _sut;

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
                                                  [$"{PitneyBowesServiceOptions.SectionName}:ApiKey"] = "test-api-key",
                                                  [$"{PitneyBowesServiceOptions.SectionName}:ApiSecret"] = "test-api-secret",
                                                  [$"{PitneyBowesServiceOptions.SectionName}:DeveloperId"] = "test-developer-id",
                                                  [$"{PitneyBowesServiceOptions.SectionName}:ClientEnvironment"] = nameof(ClientEnvironment.SANDBOX),
                                              })
                                             .Build());

        services.AddPitneyBowesAddressValidation();

        services.PostConfigure<PitneyBowesServiceOptions>(o =>
        {
            o.EndpointUriOverride = new Uri(_wireMockServer.Url!);
        });

        _serviceProvider = services.BuildServiceProvider();
        _sut = _serviceProvider.GetRequiredService<IAddressValidationService<PitneyBowesAddressValidationRequest>>();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
        _wireMockServer.Dispose();
    }

    [Test]
    public async Task ValidateAsync_WhenAddressIsResolved_ReturnsExpectedResponse(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubAddressVerify();

        PitneyBowesAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "3200 Summit Blvd",
            },
            CityOrTown = "West Palm Beach",
            StateOrProvince = "FL",
            PostalCode = "33416-4001",
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
            response.AddressLines.Should().BeEquivalentTo("3200 Summit Blvd");
            response.CityOrTown.Should().Be("West Palm Beach");
            response.StateOrProvince.Should().Be("FL");
            response.PostalCode.Should().Be("33416-4001");
            response.Country.Should().Be(CountryCode.US);
            response.IsResidential.Should().BeTrue();
            response.CustomResponseData.Should().ContainKey("carrierRoute")
                    .WhoseValue.Should().Be("C770");
            response.CustomResponseData.Should().ContainKey("deliveryPoint")
                    .WhoseValue.Should().Be("99");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenAddressValidationReturnsSuggestions_ReturnsSuggestionsInResponse(
        CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubAddressVerifySuggest();

        PitneyBowesAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "3 1st Ave NW",
            },
            CityOrTown = "Litz",
            StateOrProvince = "FL",
            PostalCode = "33549",
            Country = CountryCode.US,
            IncludeSuggestions = true,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Suggestions.Should().ContainSingle()
                    .Which.AddressLines.Should().Contain("3 1ST AVE NE");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenApiReturnsErrorResponse_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubApiErrorResponse();

        PitneyBowesAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "3200 Summit Blvd",
            },
            CityOrTown = "West Palm Beach",
            StateOrProvince = "FL",
            PostalCode = "00000",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Be("1019003: Either Invalid or missing street or stateProvince or postalcode.");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenOAuthReturnsEmptyToken_ThrowsInvalidCredentialException(
        CancellationToken cancellationToken)
    {
        _wireMockServer.Given(Request.Create().WithPath("/oauth/token").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody("""{"access_token":"","token_type":"bearer","expires_in":3600}"""));

        PitneyBowesAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "3200 Summit Blvd",
            },
            CityOrTown = "West Palm Beach",
            StateOrProvince = "FL",
            PostalCode = "33416-4001",
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
        _wireMockServer.Given(Request.Create().WithPath("/oauth/token").UsingPost())
                       .RespondWith(Response.Create().WithStatusCode(statusCode));

        PitneyBowesAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "3200 Summit Blvd",
            },
            CityOrTown = "West Palm Beach",
            StateOrProvince = "FL",
            PostalCode = "33416-4001",
            Country = CountryCode.US,
        };

        Func<Task<IAddressValidationResponse?>> act = () => _sut.ValidateAsync(request, cancellationToken);
        await act.Should().ThrowAsync<HttpRequestException>().ConfigureAwait(false);
    }

    private void StubAddressVerify()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ResolvedAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/shippingservices/v1/addresses/verify").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubAddressVerifySuggest()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "SuggestedAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/shippingservices/v1/addresses/verify-suggest").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubApiErrorResponse()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ApiErrorResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/shippingservices/v1/addresses/verify").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(400)
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
}
