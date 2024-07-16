namespace Visus.AddressValidation.Google.Tests;

using System.Net;
using AddressValidation.Abstractions;
using Http;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using Services;
using Shared.Tests.Extensions;
using Validation;

public sealed class AddressValidationServiceFacts
{
	[Fact]
	public async Task Validate_BadRequest_Logged()
	{
		var mockLogger = new Mock<ILogger<GoogleAddressValidationClient>>();

		var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "ErrorResponse.json"));

		var requestValidator = new AddressValidationRequestValidator();
		var responseValidator = new ApiAddressValidationResponseValidator();

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
		httpMessageHandlerMock.Expect("v1:validateAddress")
							  .Respond(HttpStatusCode.BadRequest, "application/json", json);

		var httpClient = httpMessageHandlerMock.ToHttpClient();

		var client = new GoogleAddressValidationClient(httpClient, mockLogger.Object);
		var service = new AddressValidationService(client, requestValidator, responseValidator);

		var response = await service.ValidateAsync(request);

		mockLogger.VerifyLoggingCall(LogLevel.Error, "BadRequest: Address is missing from request.");

		Assert.Null(response);
	}

	[Fact]
	public async Task Validate_CityState_Success()
	{
		var mockLogger = new Mock<ILogger<GoogleAddressValidationClient>>();

		var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "CityStateResponse.json"));

		var requestValidator = new AddressValidationRequestValidator();
		var responseValidator = new ApiAddressValidationResponseValidator();

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
		httpMessageHandlerMock.Expect("v1:validateAddress")
							  .WithJsonContent(request)
							  .Respond("application/json", json);

		var httpClient = httpMessageHandlerMock.ToHttpClient();

		var client = new GoogleAddressValidationClient(httpClient, mockLogger.Object);
		var service = new AddressValidationService(client, requestValidator, responseValidator);

		var response = await service.ValidateAsync(request);

		mockLogger.VerifyLoggingCall(LogLevel.Error, Times.Never());

		Assert.NotNull(response);

		Assert.Empty(response.Errors);
		Assert.Empty(response.Warnings);

		Assert.Equal(request.AddressLines.OrderBy(o => o), response.AddressLines.OrderBy(o => o));
		Assert.Null(response.CityOrTown);
		Assert.Null(response.StateOrProvince);
		Assert.Equal(request.PostalCode, request.PostalCode);
		Assert.Equal(request.Country, response.Country);
	}

	[Fact]
	public async Task Validate_Default_Success()
	{
		var mockLogger = new Mock<ILogger<GoogleAddressValidationClient>>();

		var json = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultResponse.json"));

		var requestValidator = new AddressValidationRequestValidator();
		var responseValidator = new ApiAddressValidationResponseValidator();

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
		httpMessageHandlerMock.Expect("v1:validateAddress")
							  .WithJsonContent(request)
							  .Respond("application/json", json);

		var httpClient = httpMessageHandlerMock.ToHttpClient();

		var client = new GoogleAddressValidationClient(httpClient, mockLogger.Object);
		var service = new AddressValidationService(client, requestValidator, responseValidator);

		var response = await service.ValidateAsync(request);

		mockLogger.VerifyLoggingCall(LogLevel.Error, Times.Never());

		Assert.NotNull(response);

		Assert.Empty(response.Errors);
		Assert.Empty(response.Warnings);

		Assert.Equal(request.AddressLines.OrderBy(o => o), response.AddressLines.OrderBy(o => o));
		Assert.Equal(request.CityOrTown, response.CityOrTown);
		Assert.Equal(request.StateOrProvince, response.StateOrProvince);
		Assert.Equal(request.PostalCode, request.PostalCode);
		Assert.Equal(request.Country, response.Country);
	}
}
