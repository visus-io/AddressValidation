namespace Visus.AddressValidation.Integration.Google.Tests;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using Configuration;

internal sealed class GoogleServiceOptionsTests
{
    [Test]
    public void Validate_WhenClientEnvironmentIsDevelopment_ReturnsNoErrors()
    {
        GoogleServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.DEVELOPMENT;

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsProduction_ReturnsNoErrors()
    {
        GoogleServiceOptions sut = ValidOptions();

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndAuthenticationUriOverrideIsNull_ReturnsValidationError()
    {
        GoogleServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;
        sut.EndpointUriOverride = new Uri("http://localhost:9000");

        List<ValidationResult> results = Validate(sut);

        results.Should().ContainSingle()
               .Which.MemberNames.Should().Contain(nameof(GoogleServiceOptions.AuthenticationUriOverride));
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndBothUriOverridesAreNull_ReturnsTwoValidationErrors()
    {
        GoogleServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;

        List<ValidationResult> results = Validate(sut);

        results.Should().HaveCount(2);
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndBothUriOverridesAreSet_ReturnsNoErrors()
    {
        GoogleServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;
        sut.AuthenticationUriOverride = new Uri("http://localhost:9001");
        sut.EndpointUriOverride = new Uri("http://localhost:9000");

        List<ValidationResult> results = Validate(sut);

        results.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenClientEnvironmentIsSandboxAndEndpointUriOverrideIsNull_ReturnsValidationError()
    {
        GoogleServiceOptions sut = ValidOptions();
        sut.ClientEnvironment = ClientEnvironment.SANDBOX;
        sut.AuthenticationUriOverride = new Uri("http://localhost:9001");

        List<ValidationResult> results = Validate(sut);

        results.Should().ContainSingle()
               .Which.MemberNames.Should().Contain(nameof(GoogleServiceOptions.EndpointUriOverride));
    }

    private static List<ValidationResult> Validate(GoogleServiceOptions options)
    {
        return [..options.Validate(new ValidationContext(options)),];
    }

    private static GoogleServiceOptions ValidOptions()
    {
        return new GoogleServiceOptions
        {
            ProjectId = "test-project",
            ServiceAccountEmail = "test@example.iam.gserviceaccount.com",
            PrivateKey = "test-private-key",
        };
    }
}
