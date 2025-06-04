namespace Visus.AddressValidation.Integration.Google.Tests;

using System.Net;
using AddressValidation.Abstractions;
using Http;
using RichardSzalay.MockHttp;
using Services;
using Validation;

public sealed class AddressValidationServiceFacts(ConfigurationFixture fixture) : IClassFixture<ConfigurationFixture>
{
    private readonly ConfigurationFixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task Validate_BadRequest_HasErrors()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "ErrorResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();

        // Google US
        // Request will be discarded for test
        var request = new GoogleAddressValidationRequest
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy"
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/v1:validateAddress")
                              .Respond(HttpStatusCode.BadRequest, "application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new GoogleAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);

        var response = await service.ValidateAsync(request);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Errors);
        Assert.Contains(response.Errors, error => error == "BadRequest: Address is missing from request.");

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Validate_CityState_Success()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "CityStateResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();

        // Singapore Post (North East)
        var request = new GoogleAddressValidationRequest
        {
            AddressLines =
            {
                "1 Lim Ah Pin Rd"
            },
            PostalCode = "547809",
            Country = CountryCode.SG
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/v1:validateAddress")
                              .WithJsonContent(request)
                              .Respond("application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new GoogleAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);

        var response = await service.ValidateAsync(request);

        Assert.NotNull(response);

        Assert.Empty(response.Errors);
        Assert.Empty(response.Warnings);

        Assert.Equal(request.AddressLines.OrderBy(o => o), response.AddressLines.OrderBy(o => o), StringComparer.OrdinalIgnoreCase);
        Assert.Null(response.CityOrTown);
        Assert.Null(response.StateOrProvince);
        Assert.Equal(request.PostalCode, request.PostalCode);
        Assert.Equal(request.Country, response.Country);

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Validate_Default_Success()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();

        // Google US
        var request = new GoogleAddressValidationRequest
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy"
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/v1:validateAddress")
                              .WithJsonContent(request)
                              .Respond("application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new GoogleAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);

        var response = await service.ValidateAsync(request);

        Assert.NotNull(response);

        Assert.Empty(response.Errors);
        Assert.Empty(response.Warnings);

        Assert.Equal(request.AddressLines.OrderBy(o => o), response.AddressLines.OrderBy(o => o), StringComparer.OrdinalIgnoreCase);
        Assert.Equal(request.CityOrTown, response.CityOrTown, StringComparer.OrdinalIgnoreCase);
        Assert.Equal(request.StateOrProvince, response.StateOrProvince);
        Assert.Equal(request.PostalCode, request.PostalCode);
        Assert.Equal(request.Country, response.Country);

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }
}
