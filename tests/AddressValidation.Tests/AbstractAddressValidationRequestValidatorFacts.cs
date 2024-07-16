namespace AddressValidation.Tests;

using AddressValidation.Abstractions;
using FluentValidation.TestHelper;

public sealed class AbstractAddressValidationRequestValidatorFacts
{
	[Fact]
	public void AbstractAddressValidationRequestValidator_CityState_Success()
	{
		// Singapore Post (North East)
		var request = new TestAddressValidationRequest
		{
			AddressLines =
			{
				"1 Lim Ah Pin Rd"
			},
			PostalCode = "547809",
			Country = CountryCode.SG
		};

		var validator = new TestAddressValidationRequestValidator();
		var result = validator.TestValidate(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public void AbstractAddressValidationRequestValidator_NoCountry_Fail()
	{
		// Broken Address
		var request = new TestAddressValidationRequest();

		var validator = new TestAddressValidationRequestValidator();
		var result = validator.TestValidate(request);

		result.ShouldHaveValidationErrorFor(f => f.Country);
	}

	[Fact]
	public void AbstractAddressValidationRequestValidator_Success()
	{
		// Google US
		var request = new TestAddressValidationRequest
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

		var validator = new TestAddressValidationRequestValidator();
		var result = validator.TestValidate(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public void AbstractAddressValidationRequestValidator_Unsupported_Country()
	{
		// U.S Embassy in Zimbabwe
		var request = new TestAddressValidationRequest
		{
			AddressLines =
			{
				"2 Lorraine Dr"
			},
			CityOrTown = "Harare",
			Country = CountryCode.ZW
		};

		var validator = new TestAddressValidationRequestValidator();
		var result = validator.TestValidate(request);

		result.ShouldHaveValidationErrorFor(f => f.Country);
	}

	private class TestAddressValidationRequest : AbstractAddressValidationRequest
	{
	}

	private class TestAddressValidationRequestValidator : AbstractAddressValidationRequestValidator<TestAddressValidationRequest>
	{
	}
}
