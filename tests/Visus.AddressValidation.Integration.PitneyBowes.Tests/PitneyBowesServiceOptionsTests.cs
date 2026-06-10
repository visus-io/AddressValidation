namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using AwesomeAssertions;
using Configuration;

internal sealed class PitneyBowesServiceOptionsTests
{
    [Test]
    public void Validate_WhenClientEnvironmentIsDevelopment_ReturnsNoErrors()
    {
        PitneyBowesServiceOptions sut = ValidOptions();

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsProduction_ReturnsNoErrors()
    {
        PitneyBowesServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.PRODUCTION;

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsNull_ReturnsValidationError()
    {
        PitneyBowesServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;

        List<ValidationResult> results = Validate(sut);

        results.Should().ContainSingle()
               .Which.MemberNames.Should().Contain(nameof(PitneyBowesServiceOptions.EndpointUriOverride));
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsSet_ReturnsNoErrors()
    {
        PitneyBowesServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;
        sut.EndpointUriOverride = new Uri("http://localhost:9000");

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    private static List<ValidationResult> Validate(PitneyBowesServiceOptions options)
    {
        return [..options.Validate(new ValidationContext(options)),];
    }

    private static PitneyBowesServiceOptions ValidOptions()
    {
        return new PitneyBowesServiceOptions
        {
            ApiKey = "test-api-key",
            ApiSecret = "test-api-secret",
            DeveloperId = "test-developer-id",
        };
    }
}
