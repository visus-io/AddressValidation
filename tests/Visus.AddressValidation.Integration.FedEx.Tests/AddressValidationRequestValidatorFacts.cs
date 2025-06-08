namespace Visus.AddressValidation.Integration.FedEx.Tests;

using AddressValidation.Abstractions;
using Http;
using Validation;

public sealed class AddressValidationRequestValidatorFacts
{
    [Fact]
    public async Task AddressValidationRequestValidator_Unsupported_Region()
    {
        // U.S. Embassy (Dublin, Ireland)
        var request = new FedExAddressValidationRequest
        {
            AddressLines =
            {
                "42 Elgin Rd"
            },
            CityOrTown = "Dublin",
            Country = CountryCode.IE,
        };

        var validator = new AddressValidationRequestValidator();
        var result = await validator.ExecuteAsync(request);

        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);

        Assert.Contains(result.Errors, 
                        error => error.Message == "Country: IE is currently not supported by the FedEx Address Validation API.");
    }
}
