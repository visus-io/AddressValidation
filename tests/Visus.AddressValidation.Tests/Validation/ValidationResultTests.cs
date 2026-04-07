namespace Visus.AddressValidation.Tests.Validation;

using AddressValidation.Validation;
using AutoFixture;
using AwesomeAssertions;

internal sealed class ValidationResultTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Constructor_SeparatesErrorsAndWarnings()
    {
        HashSet<ValidationState> states =
        [
            ValidationState.CreateError(_fixture.Create<string>()),
            ValidationState.CreateError(_fixture.Create<string>()),
            ValidationState.CreateWarning(_fixture.Create<string>()),
        ];

        ValidationResult result = new(states);

        result.Errors.Should().HaveCount(2);
        result.Warnings.Should().HaveCount(1);
    }

    [Test]
    public void EmptyStates_ProducesNoErrorsOrWarnings()
    {
        HashSet<ValidationState> states = [];

        ValidationResult result = new(states);

        result.HasErrors.Should().BeFalse();
        result.HasWarnings.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Warnings.Should().BeEmpty();
    }

    [Test]
    public void HasErrors_ReturnsTrue_WhenErrorsExist()
    {
        HashSet<ValidationState> states = [ValidationState.CreateError(_fixture.Create<string>()),];

        ValidationResult result = new(states);

        result.HasErrors.Should().BeTrue();
        result.HasWarnings.Should().BeFalse();
    }

    [Test]
    public void HasWarnings_ReturnsTrue_WhenWarningsExist()
    {
        HashSet<ValidationState> states = [ValidationState.CreateWarning(_fixture.Create<string>()),];

        ValidationResult result = new(states);

        result.HasWarnings.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
    }
}
