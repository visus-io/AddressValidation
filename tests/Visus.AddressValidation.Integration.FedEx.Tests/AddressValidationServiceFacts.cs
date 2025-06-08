namespace Visus.AddressValidation.Integration.FedEx.Tests;

using System.Net;
using AddressValidation.Abstractions;
using Http;
using RichardSzalay.MockHttp;
using Services;
using Validation;

public sealed class AddressValidationServiceFacts : IClassFixture<ConfigurationFixture>
{
    private readonly ConfigurationFixture _fixture;

    public AddressValidationServiceFacts(ConfigurationFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public async Task Validate_UnprocessableEntity_HasErrors()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "ErrorResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();
        
        // FedEx Ship Center (Lake Worth FL)
        // Request will be discarded for test
        var request = new FedExAddressValidationRequest
        {
            AddressLines =
            {
                "1145 Barnett Dr"
            },
            CityOrTown = "Lake Worth",
            StateOrProvince = "FL",
            PostalCode = "33461",
            Country = CountryCode.US
        };
        
        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/address/v1/addresses/resolve")
                              .WithJsonContent(request)
                              .Respond(HttpStatusCode.UnprocessableEntity, "application/json", json);
        
        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new FedExAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);
        
        var response = await service.ValidateAsync(request);
        
        Assert.NotNull(response);
        
        Assert.NotEmpty(response.Errors);
        
        Assert.Contains(response.Errors, error => error == "STANDARDIZED.ADDRESS.NOTFOUND: Standardized address is not found.");
    }

    [Fact]
    public async Task Validate_Default_Success()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();

        // FedEx Ship Center (Lake Worth FL)
        var request = new FedExAddressValidationRequest
        {
            AddressLines =
            {
                "1145 Barnett Dr"
            },
            CityOrTown = "Lake Worth",
            StateOrProvince = "FL",
            PostalCode = "33461",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/address/v1/addresses/resolve")
                              .WithJsonContent(request)
                              .Respond("application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new FedExAddressValidationClient(_fixture.Configuration, httpClient);
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
