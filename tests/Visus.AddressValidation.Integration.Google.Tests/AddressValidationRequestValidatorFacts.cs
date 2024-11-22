namespace Visus.AddressValidation.Integration.Google.Tests;

using AddressValidation.Abstractions;
using Http;
using Validation;

public sealed class AddressValidationRequestValidatorFacts
{
	[Fact]
	public async Task AddressValidationRequestValidator_Unsupported_Region()
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
		var result = await validator.ExecuteAsync(request);

		Assert.True(result.HasErrors);
		Assert.False(result.HasWarnings);

		Assert.Contains(result.Errors, error => error.Message == "Country: BY is currently not supported by the Google Address Validation API.");
	}
}
