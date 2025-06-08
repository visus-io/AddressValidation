namespace Visus.AddressValidation.Integration.FedEx.Tests;

using System.Text.Json;
using Abstractions;
using AddressValidation.Abstractions;
using Http;
using Newtonsoft.Json.Linq;

public sealed class AddressValidationRequestFacts
{
    [Fact]
    public void Serialization_CityState_Success()
    {
        var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "CityStateRequest.json"));

        // Singapore Post (North East)
        var request = new FedExAddressValidationRequest
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

        // FedEx Ship Center (Lake Worth FL)
        var request = new FedExAddressValidationRequest
        {
            AddressLines =
            {
                "1145 Barnett Dr"
            },
            CityOrTown = "Lake Worth",
            StateOrProvince = "FL",
            PostalCode = "33461",
            Country = CountryCode.US
        };

        var result = JsonSerializer.Serialize(request);

        Assert.True(JToken.DeepEquals(JToken.Parse(expected), JToken.Parse(result)));
    }
}
