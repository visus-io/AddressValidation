namespace Visus.AddressValidation.Tests.Validation;

using AddressValidation.Validation;
using AutoFixture;
using AwesomeAssertions;

internal sealed class ValidationStateTests
{
    private readonly Fixture _fixture = new();

    [Test]
    [Arguments("field is required", null, "field is required")]
    [Arguments("field is required", "PostalCode", "PostalCode: field is required")]
    public void CreateError_FormatsMessage(string message, string? propertyName, string expected)
    {
        ValidationState state = ValidationState.CreateError(message, propertyName);

        state.Message.Should().Be(expected);
        state.Severity.Should().Be(ValidationSeverity.Error);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void CreateError_NullOrWhitespaceMessage_Throws(string? message)
    {
        Action act = () => ValidationState.CreateError(message!);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void CreateError_WithCompositeFormat_FormatsMessage()
    {
        ValidationState state = ValidationState.CreateError("{0}: {1} is not supported", "Country", "ZZ");

        state.Message.Should().Be("Country: ZZ is not supported");
    }

    [Test]
    [Arguments("field is required", null, "field is required")]
    [Arguments("field is required", "PostalCode", "PostalCode: field is required")]
    public void CreateWarning_FormatsMessage(string message, string? propertyName, string expected)
    {
        ValidationState state = ValidationState.CreateWarning(message, propertyName);

        state.Message.Should().Be(expected);
        state.Severity.Should().Be(ValidationSeverity.Warning);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void CreateWarning_NullOrWhitespaceMessage_Throws(string? message)
    {
        Action act = () => ValidationState.CreateWarning(message!);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void CreateWarning_WithCompositeFormat_FormatsMessage()
    {
        ValidationState state = ValidationState.CreateWarning("{0}: {1} could not be verified", "PostalCode", "00000");

        state.Message.Should().Be("PostalCode: 00000 could not be verified");
    }

    [Test]
    public void Equals_DifferentMessage_ReturnsFalse()
    {
        ValidationState left = ValidationState.CreateError(_fixture.Create<string>());
        ValidationState right = ValidationState.CreateError(_fixture.Create<string>());

        ( left == right ).Should().BeFalse();
    }

    [Test]
    public void Equals_DifferentSeverity_ReturnsFalse()
    {
        string message = _fixture.Create<string>();
        ValidationState error = ValidationState.CreateError(message);
        ValidationState warning = ValidationState.CreateWarning(message);

        error.Equals(warning).Should().BeFalse();
    }

    [Test]
    public void Equals_IsCaseInsensitive()
    {
        string message = _fixture.Create<string>();
        ValidationState left = ValidationState.CreateError(message.ToUpperInvariant());
        ValidationState right = ValidationState.CreateError(message.ToLowerInvariant());

        left.Equals(right).Should().BeTrue();
        ( left == right ).Should().BeTrue();
        ( left != right ).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
        string message = _fixture.Create<string>();
        ValidationState left = ValidationState.CreateError(message.ToUpperInvariant());
        ValidationState right = ValidationState.CreateError(message.ToLowerInvariant());

        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Test]
    public void ToString_ReturnsMessage()
    {
        string message = _fixture.Create<string>();
        ValidationState state = ValidationState.CreateError(message);

        state.ToString().Should().Be(message);
    }
}
