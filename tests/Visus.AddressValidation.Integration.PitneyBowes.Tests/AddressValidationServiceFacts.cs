namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

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

        // USPS (West Palm Beach, FL)
        // Request will be discarded for test
        var request = new PitneyBowesAddressValidationRequest
        {
            AddressLines =
            {
                "3200 Summit Blvd"
            },
            CityOrTown = "33416",
            StateOrProvince = "FL",
            PostalCode = "West Palm Beach",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/shippingservices/v1/addresses/verify?minimalAddressValidation=false")
                              .WithHeaders("X-PB-UnifiedErrorStructure", "true")
                              .WithJsonContent(request)
                              .Respond(HttpStatusCode.BadRequest, "application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new PitneyBowesAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);

        var response = await service.ValidateAsync(request);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Errors);
        Assert.Contains(response.Errors, error => error == "1019003: Either Invalid or missing street or stateProvince or postalcode.");

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Validate_Default_Success()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();

        // USPS (West Palm Beach, FL)
        var request = new PitneyBowesAddressValidationRequest
        {
            AddressLines =
            {
                "3200 Summit Blvd"
            },
            CityOrTown = "West Palm Beach",
            StateOrProvince = "FL",
            PostalCode = "33416",
            Country = CountryCode.US
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/shippingservices/v1/addresses/verify?minimalAddressValidation=false")
                              .WithHeaders("X-PB-UnifiedErrorStructure", "true")
                              .WithJsonContent(request)
                              .Respond("application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new PitneyBowesAddressValidationClient(_fixture.Configuration, httpClient);
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

    [Fact]
    public async Task Validate_Suggest_Success()
    {
        var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "SuggestionResponse.json"));

        var requestValidator = new AddressValidationRequestValidator();
        var responseValidator = new ApiResponseValidator();

        // Sample Address from Pitney Bowes Documentation
        // https://docs.shippingapi.pitneybowes.com/api/post-address-suggest.html#returned-address-is-validated-and-not-changed
        var request = new PitneyBowesAddressValidationRequest
        {
            AddressLines =
            {
                "3 1st Ave NW"
            },
            CityOrTown = "Litz",
            StateOrProvince = "FL",
            PostalCode = "33549",
            Country = CountryCode.US,
            IncludeSuggestions = true
        };

        var httpMessageHandlerMock = new MockHttpMessageHandler();
        httpMessageHandlerMock.Expect("/shippingservices/v1/addresses/verify-suggest?returnSuggestions=true")
                              .WithHeaders("X-PB-UnifiedErrorStructure", "true")
                              .WithJsonContent(request)
                              .Respond("application/json", json);

        var httpClient = httpMessageHandlerMock.ToHttpClient();

        var client = new PitneyBowesAddressValidationClient(_fixture.Configuration, httpClient);
        var service = new AddressValidationService(client, requestValidator, responseValidator);

        var response = await service.ValidateAsync(request);

        Assert.NotNull(response);

        Assert.Empty(response.Errors);
        Assert.Empty(response.Warnings);
        Assert.NotEmpty(response.Suggestions);

        Assert.Contains(response.Suggestions,
                        suggestion =>
                            suggestion.AddressLines.Contains("3 1ST AVE NE", StringComparer.OrdinalIgnoreCase));

        httpMessageHandlerMock.VerifyNoOutstandingExpectation();
    }
}
