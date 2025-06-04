namespace Visus.AddressValidation.Integration.Ups.Tests;

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

        // UPS Distribution Center (Mechanicsville VA)
        var request = new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "8380 Richfood Rd"
            },
            CityOrTown = "Mechanicsville",
            StateOrProvince = "VA",
            PostalCode = "23116",
            Country = CountryCode.US
        };

        var result = JsonSerializer.Serialize(request);

        Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
    }

    [Fact]
    public void Serialization_ZipPlusFour_Success()
    {
        var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "ZipPlusFourRequest.json"));

        // UPS Distribution Center (Mechanicsville VA)
        var request = new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "8380 Richfood Rd"
            },
            CityOrTown = "Mechanicsville",
            StateOrProvince = "VA",
            PostalCode = "23116-2010",
            Country = CountryCode.US
        };

        var result = JsonSerializer.Serialize(request);

        Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
    }
}
