namespace Visus.AddressValidation.Tests.Validation;

using Abstractions;
using AddressValidation.Validation;
using AutoFixture;
using AwesomeAssertions;
using Http;
using Services;

internal sealed class AbstractAddressValidationRequestValidatorTests
{
    private readonly Fixture _fixture = new();

    private TestAddressValidationRequest CreateValidRequest()
    {
        TestAddressValidationRequest request = new()
        {
            Country = CountryCode.US,
            CityOrTown = _fixture.Create<string>(),
            StateOrProvince = _fixture.Create<string>(),
            PostalCode = _fixture.Create<string>(),
        };
        request.AddressLines.Add(_fixture.Create<string>());
        return request;
    }

    [Test]
    public async Task PreValidate_NullCountry_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = new();

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    [Arguments(CountryCode.US)]
    [Arguments(CountryCode.CA)]
    public async Task PreValidate_SupportedCountry_PassesPreValidation(CountryCode country)
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.Country = country;

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeFalse();
    }

    [Test]
    [Arguments(CountryCode.DE)]
    [Arguments(CountryCode.FR)]
    [Arguments(CountryCode.JP)]
    public async Task PreValidate_UnsupportedCountry_HasErrors(CountryCode country)
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = new()
        {
            Country = country,
        };

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    public async Task Validate_EmptyAddressLines_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.AddressLines.Clear();

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    public async Task Validate_MissingCityOrTown_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.CityOrTown = null;

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    public async Task Validate_MissingPostalCode_ForCountryWithPostalCodes_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.PostalCode = null;

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    public async Task Validate_MissingStateOrProvince_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.StateOrProvince = null;

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    public async Task Validate_MoreThanThreeAddressLines_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.AddressLines.Add(_fixture.Create<string>());
        request.AddressLines.Add(_fixture.Create<string>());
        request.AddressLines.Add(_fixture.Create<string>());

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }

    [Test]
    public async Task Validate_ValidRequest_NoErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WhitespaceOnlyAddressLines_HasErrors()
    {
        TestRequestValidator validator = new();
        TestAddressValidationRequest request = CreateValidRequest();
        request.AddressLines.Clear();
        request.AddressLines.Add("   ");

        IValidationResult result = await validator.ExecuteAsync(request).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
    }
}
