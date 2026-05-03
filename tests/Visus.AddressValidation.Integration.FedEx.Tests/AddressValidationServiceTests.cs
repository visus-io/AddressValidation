namespace Visus.AddressValidation.Integration.FedEx.Tests;

using System.Security.Authentication;
using AddressValidation.Abstractions;
using AddressValidation.Models;
using AddressValidation.Services;
using AwesomeAssertions;
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

    private readonly IAddressValidationService<FedExAddressValidationRequest> _sut;

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
                                                  [$"{FedExServiceOptions.SectionName}:ClientId"] = "test-client-id",
                                                  [$"{FedExServiceOptions.SectionName}:ClientSecret"] = "test-client-secret",
                                                  [$"{FedExServiceOptions.SectionName}:AccountNumber"] = "test-account",
                                                  [$"{FedExServiceOptions.SectionName}:ClientEnvironment"] = nameof(ClientEnvironment.SANDBOX),
                                              })
                                             .Build());

        services.AddFedExAddressValidation();

        services.PostConfigure<FedExServiceOptions>(o =>
        {
            o.EndpointOverrideUri = new Uri(_wireMockServer.Url!);
        });

        _serviceProvider = services.BuildServiceProvider();
        _sut = _serviceProvider.GetRequiredService<IAddressValidationService<FedExAddressValidationRequest>>();
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
        StubAddressResolve();

        FedExAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "7372 PARKRIDGE BLVD",
                "APT 286",
            },
            CityOrTown = "IRVING",
            StateOrProvince = "TX",
            PostalCode = "75063-8659",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        response.Should().NotBeNull();
        response.Errors.Should().BeEmpty();
        response.Warnings.Should().BeEmpty();
        response.Suggestions.Should().BeEmpty();
        response.AddressLines.Should().BeEquivalentTo("7372 PARKRIDGE BLVD", "APT 286");
        response.CityOrTown.Should().Be("IRVING");
        response.StateOrProvince.Should().Be("TX");
        response.PostalCode.Should().Be("75063-8365");
        response.Country.Should().Be(CountryCode.US);
        response.IsResidential.Should().BeFalse();
        response.CustomResponseData.Should().ContainKey("customerTransactionId")
                .WhoseValue.Should().Be("APIF_SV_ADVC_TxIDcustomer_test");
        response.CustomResponseData.Should().ContainKey("matchSource")
                .WhoseValue.Should().Be("Postal");
    }

    [Test]
    public async Task ValidateAsync_WhenAddressIsInterpolated_ReturnsWarning(CancellationToken cancellationToken)
    {
        StubOAuthToken();
        StubInterpolatedStreetAddressResolve();

        FedExAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "7372 PARKRIDGE BLVD",
                "APT 286",
            },
            CityOrTown = "IRVING",
            StateOrProvince = "TX",
            PostalCode = "75063-8659",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        response.Should().NotBeNull();
        response.Errors.Should().BeEmpty();
        response.Warnings.Should().ContainSingle()
                .Which.Should().Be("INTERPOLATED.STREET.ADDRESS: There is a chance that the address is not valid.");
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

        FedExAddressValidationRequest request = new()
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

        FedExAddressValidationRequest request = new()
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

        Func<Task<IAddressValidationResponse?>> act = () => _sut.ValidateAsync(request, cancellationToken);
        await act.Should().ThrowAsync<HttpRequestException>().ConfigureAwait(false);
    }

    private void StubAddressResolve()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ResolvedAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/address/v1/addresses/resolve").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubInterpolatedStreetAddressResolve()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "InterpolatedStreetAddressResponse.json"));

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
}
