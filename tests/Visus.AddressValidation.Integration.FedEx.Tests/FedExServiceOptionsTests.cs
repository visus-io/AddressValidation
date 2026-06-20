namespace Visus.AddressValidation.Integration.FedEx.Tests;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using Configuration;

internal sealed class FedExServiceOptionsTests
{
    [Test]
    public void Validate_WhenClientEnvironmentIsDevelopment_ReturnsNoErrors()
    {
        FedExServiceOptions sut = ValidOptions();

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsProduction_ReturnsNoErrors()
    {
        FedExServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.PRODUCTION;

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsNull_ReturnsValidationError()
    {
        FedExServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;

        List<ValidationResult> results = Validate(sut);

        results.Should().ContainSingle()
               .Which.MemberNames.Should().Contain(nameof(FedExServiceOptions.EndpointUriOverride));
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsSet_ReturnsNoErrors()
    {
        FedExServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;
        sut.EndpointUriOverride = new Uri("http://localhost:9000");

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenLocaleIsUnsupportedBcp47Tag_ReturnsValidationError()
    {
        FedExServiceOptions sut = ValidOptions();
        sut.Locale = "xx-XX";

        List<ValidationResult> results = Validate(sut);

        results.Should().ContainSingle()
               .Which.MemberNames.Should().Contain(nameof(FedExServiceOptions.Locale));
    }

    [Test]
    public void Validate_WhenLocaleIsValidBcp47Tag_ReturnsNoErrors()
    {
        FedExServiceOptions sut = ValidOptions();
        sut.Locale = "en-US";

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    private static List<ValidationResult> Validate(FedExServiceOptions options)
    {
        return [..options.Validate(new ValidationContext(options)),];
    }

    private static FedExServiceOptions ValidOptions()
    {
        return new FedExServiceOptions
        {
            AccountNumber = "test-account",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
        };
    }
}
