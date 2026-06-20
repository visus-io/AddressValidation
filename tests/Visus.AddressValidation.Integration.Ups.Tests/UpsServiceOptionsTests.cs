namespace Visus.AddressValidation.Integration.Ups.Tests;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using Configuration;

internal sealed class UpsServiceOptionsTests
{
    [Test]
    public void Validate_WhenClientEnvironmentIsDevelopment_ReturnsNoErrors()
    {
        UpsServiceOptions sut = ValidOptions();

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsProduction_ReturnsNoErrors()
    {
        UpsServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.PRODUCTION;

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsNull_ReturnsValidationError()
    {
        UpsServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;

        List<ValidationResult> results = Validate(sut);

        results.Should().ContainSingle()
               .Which.MemberNames.Should().Contain(nameof(UpsServiceOptions.EndpointUriOverride));
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsSet_ReturnsNoErrors()
    {
        UpsServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;
        sut.EndpointUriOverride = new Uri("http://localhost:9000");

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    private static List<ValidationResult> Validate(UpsServiceOptions options)
    {
        return [..options.Validate(new ValidationContext(options)),];
    }

    private static UpsServiceOptions ValidOptions()
    {
        return new UpsServiceOptions
        {
            AccountNumber = "test-account-number",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
        };
    }
}
