namespace Visus.AddressValidation.Integration.Ups.Tests;

using AddressValidation.Abstractions;
using Http;
using Validation;

public sealed class AddressValidationRequestValidatorFacts(ConfigurationFixture fixture) : IClassFixture<ConfigurationFixture>
{
    private readonly ConfigurationFixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task AddressValidationRequestValidator_Unsupported_Region()
    {
        // U.S. Embassy (Toronto, Canada)
        var request = new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "225 Simcoe St"
            },
            CityOrTown = "Toronto",
            StateOrProvince = "ON",
            Country = CountryCode.CA
        };

        var validator = new AddressValidationRequestValidator(_fixture.Configuration);
        var result = await validator.ExecuteAsync(request);

        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);

        Assert.Contains(result.Errors, error => error.Message == "Country: CA is currently not supported by the UPS Address Validation API.");
    }
}
