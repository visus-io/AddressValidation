namespace Visus.AddressValidation.Integration.Google.Tests;

using System.Text.Json;
using AddressValidation.Abstractions;
using Http;
using Newtonsoft.Json.Linq;

public sealed class AddressValidationRequestFacts
{
    [Fact]
    public void Deserialization_Default_Success()
    {
        var expected = new GoogleAddressValidationRequest
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

        var source = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultRequest.json"));

        var result = JsonSerializer.Deserialize<GoogleAddressValidationRequest>(source);

        Assert.Equivalent(expected, result, true);
    }

    [Fact]
    public void Serialization_CityState_Success()
    {
        var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "CityStateRequest.json"));

        // Singapore Post (North East)
        var request = new GoogleAddressValidationRequest
        {
            AddressLines =
            {
                "1 Lim Ah Pin Road"
            },
            PostalCode = "547809",
            Country = CountryCode.SG
        };

        var result = JsonSerializer.Serialize(request);

        Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
    }

    [Fact]
    public void Serialization_Default_Success()
    {
        var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "DefaultRequest.json"));

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

        var result = JsonSerializer.Serialize(request);

        Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
    }

    [Fact]
    public void Serialization_NoUspsCass_Success()
    {
        var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "NoUspsCassRequest.json"));

        // Google Canada
        var request = new GoogleAddressValidationRequest
        {
            AddressLines =
            {
                "111 Richmond Street West"
            },
            CityOrTown = "Toronto",
            StateOrProvince = "ON",
            PostalCode = "M5H 2G4",
            Country = CountryCode.CA
        };

        var result = JsonSerializer.Serialize(request);

        Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
    }
}
