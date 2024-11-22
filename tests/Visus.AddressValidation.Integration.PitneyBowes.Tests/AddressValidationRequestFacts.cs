namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using System.Text.Json;
using AddressValidation.Abstractions;
using Http;
using Newtonsoft.Json.Linq;

public sealed class AddressValidationRequestFacts
{
	[Fact]
	public void Serialization_Default_Success()
	{
		var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultRequest.json"));

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

		var result = JsonSerializer.Serialize(request);

		Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
	}
}
