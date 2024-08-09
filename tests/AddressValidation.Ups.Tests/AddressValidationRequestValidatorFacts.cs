namespace Visus.AddressValidation.Ups.Tests;

using Abstractions;
using FluentValidation.TestHelper;
using Http;
using Validation;

public sealed class AddressValidationRequestValidatorFacts
{
	[Fact]
	public void AddressValidationRequestValidator_Unsupported_Region()
	{
		// U.S. Embassy (Toronto, Canada)
		var request = new UpsAddressValidationRequest
		{
			AddressLines =
			{
				"225 Simcoe St"
			},
			CityOrTown = "Toronto",
			StateOrProvince = "ON",
			Country = CountryCode.CA
		};

		var validator = new AddressValidationRequestValidator();
		var result = validator.TestValidate(request);

		result.ShouldHaveValidationErrorFor(f => f.Country);
	}
}
