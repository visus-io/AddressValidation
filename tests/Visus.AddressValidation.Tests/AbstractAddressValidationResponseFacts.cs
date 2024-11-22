namespace Visus.AddressValidation.Tests;

using AddressValidation.Abstractions;
using Model;

public sealed class AbstractAddressValidationResponseFacts
{
	[Fact]
	public void AbstractAddressValidationResponse_Equals()
	{
		// Google US
		var left = new TestAddressValidationResponse
		{
			AddressLines = new HashSet<string>
			{
				"1600 Amphitheatre Pkwy"
			},
			CityOrTown = "Mountain View",
			StateOrProvince = "CA",
			PostalCode = "94043",
			Country = CountryCode.US
		};

		var right = new TestAddressValidationResponse
		{
			AddressLines = new HashSet<string>
			{
				"1600 Amphitheatre Pkwy"
			},
			CityOrTown = "Mountain View",
			StateOrProvince = "CA",
			PostalCode = "94043",
			Country = CountryCode.US
		};

		Assert.True(left.Equals(right));
		Assert.True(left.Equals((object) right));

		Assert.True(left == right);

		Assert.True(left.GetHashCode() == right.GetHashCode());
	}

	[Fact]
	public void AbstractAddressValidationResponse_NotEquals()
	{
		// Google US
		var left = new TestAddressValidationResponse
		{
			AddressLines = new HashSet<string>
			{
				"1600 Amphitheatre Pkwy"
			},
			CityOrTown = "Mountain View",
			StateOrProvince = "CA",
			PostalCode = "94043",
			Country = CountryCode.US
		};

		// UPS Distribution Center (Mechanicsville VA)
		var right = new TestAddressValidationResponse
		{
			AddressLines = new HashSet<string>
			{
				"8380 Richfood Rd"
			},
			CityOrTown = "Mechanicsville",
			StateOrProvince = "VA",
			PostalCode = "23116",
			Country = CountryCode.US
		};

		Assert.False(left.Equals(right));
		Assert.False(left.Equals((object) right));

		Assert.True(left != right);

		Assert.False(left.GetHashCode() == right.GetHashCode());
	}

	private sealed class TestAddressValidationResponse : AbstractAddressValidationResponse;
}
