namespace Visus.AddressValidation.Integration.Google.Tests;

using System.Security.Authentication;
using System.Security.Cryptography;
using AddressValidation.Abstractions;
using AddressValidation.Models;
using AddressValidation.Services;
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

    private readonly IAddressValidationService<GoogleAddressValidationRequest> _sut;

    private readonly WireMockServer _wireMockServer;

    public AddressValidationServiceTests()
    {
        _wireMockServer = WireMockServer.Start();

        string privateKey;
        using ( RSA rsa = RSA.Create(2048) )
        {
            byte[] pkcs8 = rsa.ExportPkcs8PrivateKey();
            privateKey = $"-----BEGIN PRIVATE KEY-----\n{Convert.ToBase64String(pkcs8)}\n-----END PRIVATE KEY-----";
        }

        ServiceCollection services = [];

        services.AddLogging()
                .AddHybridCache();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                                             .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                                              {
                                                  [$"{GoogleServiceOptions.SectionName}:ProjectId"] = "test-project-id",
                                                  [$"{GoogleServiceOptions.SectionName}:ServiceAccountEmail"] = "test@test-project.iam.gserviceaccount.com",
                                                  [$"{GoogleServiceOptions.SectionName}:PrivateKey"] = privateKey,
                                                  [$"{GoogleServiceOptions.SectionName}:ClientEnvironment"] = nameof(ClientEnvironment.SANDBOX),
                                              })
                                             .Build());

        services.AddGoogleAddressValidation();

        services.PostConfigure<GoogleServiceOptions>(o =>
        {
            o.EndpointUriOverride = new Uri(_wireMockServer.Url!);
            o.AuthenticationUriOverride = new Uri(_wireMockServer.Url! + "/token");
        });

        _serviceProvider = services.BuildServiceProvider();
        _sut = _serviceProvider.GetRequiredService<IAddressValidationService<GoogleAddressValidationRequest>>();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
        _wireMockServer.Dispose();
    }

    [Test]
    public async Task ValidateAsync_WhenAddressHasInferredComponents_ReturnsWarning(CancellationToken cancellationToken)
    {
        StubAuthToken();
        StubResolvedAddressValidate();

        GoogleAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy",
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Warnings.Should().ContainSingle()
                    .Which.Should().Be("PARTIALLY_VERIFIED");
            response.CustomResponseData.Should().ContainKey("dpvConfirmation")
                    .WhoseValue.Should().Be("Y");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenAddressIsResolved_ReturnsExpectedResponse(CancellationToken cancellationToken)
    {
        StubAuthToken();
        StubNoUspsCassAddressValidate();

        GoogleAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "111 Richmond Street West",
            },
            CityOrTown = "Toronto",
            StateOrProvince = "ON",
            PostalCode = "M5H 2G4",
            Country = CountryCode.CA,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Warnings.Should().BeEmpty();
            response.Suggestions.Should().BeEmpty();
            response.AddressLines.Should().BeEquivalentTo("111 Richmond St W");
            response.CityOrTown.Should().Be("Toronto");
            response.StateOrProvince.Should().Be("ON");
            response.PostalCode.Should().Be("M5H 2G4");
            response.Country.Should().Be(CountryCode.CA);
            response.IsResidential.Should().BeFalse();
            response.CustomResponseData.Should().ContainKey("responseId");
            response.CustomResponseData.Should().ContainKey("googlePlaceId")
                    .WhoseValue.Should().Be("ChIJv6vnls00K4gRqb4bbZ2XqfE");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenApiReturnsErrorResponse_ReturnsResponseWithErrors(CancellationToken cancellationToken)
    {
        StubAuthToken();
        StubApiErrorResponse();

        GoogleAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy",
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().ContainSingle()
                    .Which.Should().Be("BadRequest: Address is missing from request.");
        }
    }

    [Test]
    public async Task ValidateAsync_WhenAuthReturnsEmptyToken_ThrowsInvalidCredentialException(
        CancellationToken cancellationToken)
    {
        _wireMockServer.Given(Request.Create().WithPath("/token").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody("""{"access_token":"","token_type":"bearer","expires_in":3600}"""));

        GoogleAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy",
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US,
        };

        Func<Task<IAddressValidationResponse?>> act = () => _sut.ValidateAsync(request, cancellationToken);
        await act.Should().ThrowAsync<InvalidCredentialException>().ConfigureAwait(false);
    }

    [Test]
    [Arguments(401)]
    [Arguments(403)]
    [Arguments(500)]
    public async Task ValidateAsync_WhenAuthTokenRequestFails_ThrowsHttpRequestException(
        int statusCode, CancellationToken cancellationToken)
    {
        _wireMockServer.Given(Request.Create().WithPath("/token").UsingPost())
                       .RespondWith(Response.Create().WithStatusCode(statusCode));

        GoogleAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy",
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US,
        };

        Func<Task<IAddressValidationResponse?>> act = () => _sut.ValidateAsync(request, cancellationToken);
        await act.Should().ThrowAsync<HttpRequestException>().ConfigureAwait(false);
    }

    [Test]
    public async Task ValidateAsync_WhenInternationalAddressHasNoStateOrProvince_ReturnsExpectedResponse(
        CancellationToken cancellationToken)
    {
        StubAuthToken();
        StubCityStateAddressValidate();

        GoogleAddressValidationRequest request = new()
        {
            AddressLines =
            {
                "1 Lim Ah Pin Road",
            },
            PostalCode = "547809",
            Country = CountryCode.SG,
        };

        IAddressValidationResponse? response = await _sut.ValidateAsync(request, cancellationToken)
                                                         .ConfigureAwait(false);

        using ( new AssertionScope() )
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
            response.Warnings.Should().BeEmpty();
            response.AddressLines.Should().BeEquivalentTo("1 Lim Ah Pin Rd");
            response.CityOrTown.Should().BeNull();
            response.StateOrProvince.Should().BeNull();
            response.PostalCode.Should().Be("547809");
            response.Country.Should().Be(CountryCode.SG);
            response.IsResidential.Should().BeNull();
        }
    }

    private void StubApiErrorResponse()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ApiErrorResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/v1:validateAddress").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(400)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubAuthToken()
    {
        _wireMockServer.Given(Request.Create().WithPath("/token").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody("""{"access_token":"test-token","token_type":"bearer","expires_in":3600}"""));
    }

    private void StubCityStateAddressValidate()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "CityStateResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/v1:validateAddress").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubNoUspsCassAddressValidate()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "NoUspsCassResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/v1:validateAddress").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }

    private void StubResolvedAddressValidate()
    {
        string body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "ResolvedAddressResponse.json"));

        _wireMockServer.Given(Request.Create().WithPath("/v1:validateAddress").UsingPost())
                       .RespondWith(Response.Create()
                                            .WithStatusCode(200)
                                            .WithHeader("Content-Type", "application/json")
                                            .WithBody(body));
    }
}
