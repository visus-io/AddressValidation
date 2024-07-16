namespace Visus.AddressValidation.Google.Tests;

using AddressValidation.Abstractions;
using FluentValidation.TestHelper;
using Http;
using Validation;

public sealed class AddressValidationRequestValidatorFacts
{
	[Fact]
	public void AddressValidationRequestValidator_Unsupported_Region()
	{
		// U.S. Embassy (Belarus)
		var request = new GoogleAddressValidationRequest
		{
			AddressLines =
			{
				"Ulitsa Starovilenskaya 46"
			},
			CityOrTown = "Minsk",
			StateOrProvince = "Minskaja voblasć",
			Country = CountryCode.BY
		};

		var validator = new AddressValidationRequestValidator();
		var result = validator.TestValidate(request);

		result.ShouldHaveValidationErrorFor(f => f.Country);
	}
}
