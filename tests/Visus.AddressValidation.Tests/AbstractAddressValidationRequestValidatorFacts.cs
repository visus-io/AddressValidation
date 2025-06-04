namespace Visus.AddressValidation.Tests;

using AddressValidation.Abstractions;
using AddressValidation.Http;
using Validation;

public sealed class AbstractAddressValidationRequestValidatorFacts
{
    [Fact]
    public async Task AbstractAddressValidationRequestValidator_CityState_Success()
    {
        // Singapore Post (North East)
        var request = new TestAddressValidationRequest
        {
            AddressLines =
            {
                "1 Lim Ah Pin Rd"
            },
            PostalCode = "547809",
            Country = CountryCode.SG
        };

        var validator = new TestAddressValidationRequestValidator();
        var result = await validator.ExecuteAsync(request);

        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
    }

    [Fact]
    public async Task AbstractAddressValidationRequestValidator_NoCountry_Fail()
    {
        // Broken Address
        var request = new TestAddressValidationRequest();

        var validator = new TestAddressValidationRequestValidator();
        var result = await validator.ExecuteAsync(request);

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, error => error.Message == "Country: Value cannot be null.");
    }

    [Fact]
    public async Task AbstractAddressValidationRequestValidator_Success()
    {
        // Google US
        var request = new TestAddressValidationRequest
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

        var validator = new TestAddressValidationRequestValidator();
        var result = await validator.ExecuteAsync(request);

        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
    }

    [Fact]
    public async Task AbstractAddressValidationRequestValidator_Unsupported_Country()
    {
        // U.S Embassy in Zimbabwe
        var request = new TestAddressValidationRequest
        {
            AddressLines =
            {
                "2 Lorraine Dr"
            },
            CityOrTown = "Harare",
            Country = CountryCode.ZW
        };

        var validator = new TestAddressValidationRequestValidator();
        var result = await validator.ExecuteAsync(request);

        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Contains(result.Errors, error => error.Message == "Country: ZW is currently not supported for address validation.");
    }

    private sealed class TestAddressValidationRequest : AbstractAddressValidationRequest;

    private class TestAddressValidationRequestValidator : AbstractAddressValidationRequestValidator<TestAddressValidationRequest>;
}
