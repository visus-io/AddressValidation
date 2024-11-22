namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using AddressValidation.Abstractions;
using Http;
using Validation;

public sealed class AddressValidationRequestValidatorFacts
{
	[Fact]
	public async Task AddressValidationRequestValidator_Unsupported_Region()
	{
		// U.S. Embassy (Toronto, Canada)
		var request = new PitneyBowesAddressValidationRequest
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
		var result = await validator.ExecuteAsync(request);

		Assert.True(result.HasErrors);
		Assert.False(result.HasWarnings);

		Assert.Contains(result.Errors, error => error.Message == "Country: CA is currently not supported by the Pitney Bowes Address Validation API.");
	}
}
