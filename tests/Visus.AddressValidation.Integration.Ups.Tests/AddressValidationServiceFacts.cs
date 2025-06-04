namespace Visus.AddressValidation.Integration.Ups.Tests;

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

        var requestValidator = new AddressValidationRequestValidator(_fixture.Configuration);
        var responseValidator = new ApiResponseValidator();

        // UPS Distribution Center (Mechanicsville, VA)
        // Request will be discarded for test
        var request = new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "8380 Richfood Rd"
            },
            CityOrTown = "23116",
            StateOrProvince = "VA",
            PostalCode = "Mechanicsville",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/api/addressvalidation/v2/3")
                              .WithJsonContent(request)
                              .Respond(HttpStatusCode.BadRequest, "application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new UpsAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);

        var response = await service.ValidateAsync(request);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Errors);
        Assert.Contains(response.Errors, error => error == "10002: The request is well formed but the request is not valid");

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Validate_Default_Success()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultResponse.json"));

        var requestValidator = new AddressValidationRequestValidator(_fixture.Configuration);
        var responseValidator = new ApiResponseValidator();

        // UPS Distribution Center (Mechanicsville, VA)
        var request = new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "8380 Richfood Rd"
            },
            CityOrTown = "Mechanicsville",
            StateOrProvince = "VA",
            PostalCode = "23116",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/api/addressvalidation/v2/3")
                              .WithJsonContent(request)
                              .Respond("application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new UpsAddressValidationClient(_fixture.Configuration, httpClient);
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
