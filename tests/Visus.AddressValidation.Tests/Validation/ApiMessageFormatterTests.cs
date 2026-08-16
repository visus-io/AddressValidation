namespace Visus.AddressValidation.Tests.Validation;

using AddressValidation.Validation;

internal sealed class ApiMessageFormatterTests
{
    [Test]
    public void CreateError_CodeAndMessage_ReturnsCodeColonMessage()
    {
        ValidationState state = ApiMessageFormatter.CreateError("ERR001", "Something went wrong.");

        state.Severity.Should().Be(ValidationSeverity.ERROR);
        state.Message.Should().Be("ERR001: Something went wrong.");
    }

    [Test]
    public void CreateError_EmptyCode_ReturnsBareMessage()
    {
        ValidationState state = ApiMessageFormatter.CreateError(string.Empty, "Something went wrong.");

        state.Message.Should().Be("Something went wrong.");
    }

    [Test]
    public void CreateError_NullCode_ReturnsBareMessage()
    {
        ValidationState state = ApiMessageFormatter.CreateError(null, "Something went wrong.");

        state.Message.Should().Be("Something went wrong.");
    }

    [Test]
    public void CreateError_NullCodeAndMessage_Throws()
    {
        Action act = static () => ApiMessageFormatter.CreateError(null, null);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void CreateError_WhitespaceMessage_ReturnsBareCode()
    {
        ValidationState state = ApiMessageFormatter.CreateError("ERR001", "   ");

        state.Message.Should().Be("ERR001");
    }

    [Test]
    public void CreateWarning_CodeAndMessage_ReturnsCodeColonMessage()
    {
        ValidationState state = ApiMessageFormatter.CreateWarning("WARN001", "Heads up.");

        state.Severity.Should().Be(ValidationSeverity.WARNING);
        state.Message.Should().Be("WARN001: Heads up.");
    }

    [Test]
    public void CreateWarning_EmptyCode_ReturnsBareMessage()
    {
        ValidationState state = ApiMessageFormatter.CreateWarning(string.Empty, "Heads up.");

        state.Message.Should().Be("Heads up.");
    }

    [Test]
    public void CreateWarning_NullCode_ReturnsBareMessage()
    {
        ValidationState state = ApiMessageFormatter.CreateWarning(null, "Heads up.");

        state.Message.Should().Be("Heads up.");
    }

    [Test]
    public void CreateWarning_NullCodeAndMessage_Throws()
    {
        Action act = static () => ApiMessageFormatter.CreateWarning(null, null);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void CreateWarning_NullMessage_ReturnsBareCode()
    {
        ValidationState state = ApiMessageFormatter.CreateWarning("WARN001", null);

        state.Message.Should().Be("WARN001");
    }
}
