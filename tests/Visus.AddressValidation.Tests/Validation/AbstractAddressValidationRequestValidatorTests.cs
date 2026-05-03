namespace Visus.AddressValidation.Tests.Validation;

using System.Collections.Frozen;
using Abstractions;
using AddressValidation.Validation;
using AwesomeAssertions;
using Models;

internal sealed class AbstractAddressValidationRequestValidatorTests
{
    private readonly TestValidator _validator = new();

    [Test]
    public async Task AddressLines_AllWhitespace_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = ValidUsRequest();
        request.AddressLines.Clear();
        request.AddressLines.Add("   ");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.AddressLines)));
    }

    [Test]
    public async Task AddressLines_Empty_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = ValidUsRequest();
        request.AddressLines.Clear();

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.AddressLines)));
    }

    [Test]
    public async Task AddressLines_ExceedsThree_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = ValidUsRequest();
        request.AddressLines.Add("Line 2");
        request.AddressLines.Add("Line 3");
        request.AddressLines.Add("Line 4");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.AddressLines)));
    }

    [Test]
    public async Task CityOrTown_MissingForNonCityState_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = ValidUsRequest();
        request.CityOrTown = null;

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.CityOrTown)));
    }

    [Test]
    public async Task CityOrTownAndStateOrProvince_CityStateCountry_NoError(CancellationToken cancellationToken)
    {
        TestRequest request = new()
        {
            Country = CountryCode.SG,
        };
        request.AddressLines.Add("1 Orchard Rd");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().NotContain(e => e.Message.Contains(nameof(request.CityOrTown)));
        result.Errors.Should().NotContain(e => e.Message.Contains(nameof(request.StateOrProvince)));
    }

    [Test]
    public async Task NoPostalCodeFallback_MissingForNoPostalCodeCountry_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = new()
        {
            Country = CountryCode.AE,
            CityOrTown = "Dubai",
            StateOrProvince = "DU",
        };
        request.AddressLines.Add("1 Sheikh Zayed Rd");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.Country)));
    }

    [Test]
    public async Task NoPostalCodeFallback_ProvidedForNoPostalCodeCountry_NoError(CancellationToken cancellationToken)
    {
        TestRequestWithFallback request = new()
        {
            Country = CountryCode.AE,
            CityOrTown = "Dubai",
            StateOrProvince = "DU",
        };
        request.AddressLines.Add("1 Sheikh Zayed Rd");

        TestValidatorForFallback validator = new();
        IValidationResult result = await validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public async Task PostalCode_MissingForNoPostalCodeCountry_NoPostalCodeError(CancellationToken cancellationToken)
    {
        TestRequest request = new()
        {
            Country = CountryCode.AE,
            CityOrTown = "Dubai",
            StateOrProvince = "DU",
        };
        request.AddressLines.Add("1 Sheikh Zayed Rd");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().NotContain(e => e.Message.Contains(nameof(request.PostalCode)));
    }

    [Test]
    public async Task PostalCode_MissingForPostalCodeCountry_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = ValidUsRequest();
        request.PostalCode = null;

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.PostalCode)));
    }

    [Test]
    public async Task PreValidate_NullCountry_ReturnsErrorAndSkipsAddressValidation(CancellationToken cancellationToken)
    {
        TestRequest request = new()
        {
            CityOrTown = "City",
            StateOrProvince = "ST",
            PostalCode = "12345",
        };
        request.AddressLines.Add("123 Main St");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains(nameof(request.Country)));
        result.Errors.Should().NotContain(e => e.Message.Contains(nameof(request.AddressLines)));
    }

    [Test]
    public async Task PreValidate_UnsupportedCountry_ReturnsErrorWithProviderNameAndSkipsAddressValidation(CancellationToken cancellationToken)
    {
        TestRequest request = new()
        {
            Country = CountryCode.DE,
            CityOrTown = "City",
            StateOrProvince = "ST",
            PostalCode = "12345",
        };
        request.AddressLines.Add("123 Main St");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains(TestValidator.Name));
        result.Errors.Should().NotContain(e => e.Message.Contains(nameof(request.AddressLines)));
    }

    [Test]
    public async Task StateOrProvince_MissingForNonCityState_ReturnsError(CancellationToken cancellationToken)
    {
        TestRequest request = ValidUsRequest();
        request.StateOrProvince = null;

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.Errors.Should().Contain(e => e.Message.Contains(nameof(request.StateOrProvince)));
    }

    [Test]
    public async Task ValidRequest_CityState_NoErrors(CancellationToken cancellationToken)
    {
        TestRequest request = new()
        {
            Country = CountryCode.SG,
            PostalCode = "238823",
        };
        request.AddressLines.Add("1 Orchard Rd");

        IValidationResult result = await _validator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public async Task ValidRequest_Us_NoErrors(CancellationToken cancellationToken)
    {
        IValidationResult result = await _validator.ExecuteAsync(ValidUsRequest(), cancellationToken).ConfigureAwait(false);

        result.HasErrors.Should().BeFalse();
    }

    private static TestRequest ValidUsRequest()
    {
        TestRequest request = new()
        {
            Country = CountryCode.US,
            CityOrTown = "Springfield",
            StateOrProvince = "IL",
            PostalCode = "62701",
        };
        request.AddressLines.Add("123 Main St");
        return request;
    }

    private sealed class TestRequest : AbstractAddressValidationRequest;

    private sealed class TestRequestWithFallback : AbstractAddressValidationRequest
    {
        public override string? NoPostalCodeFallback => "NOPCODE";
    }

    private sealed class TestValidator : AbstractAddressValidationRequestValidator<TestRequest>
    {
        internal const string Name = "Test";

        protected override string ProviderName => Name;

        protected override FrozenSet<CountryCode> SupportedCountries =>
            new[]
            {
                CountryCode.US,
                CountryCode.SG,
                CountryCode.AE,
            }.ToFrozenSet();
    }

    private sealed class TestValidatorForFallback : AbstractAddressValidationRequestValidator<TestRequestWithFallback>
    {
        protected override string ProviderName => "Test";

        protected override FrozenSet<CountryCode> SupportedCountries =>
            new[]
            {
                CountryCode.AE,
            }.ToFrozenSet();
    }
}
