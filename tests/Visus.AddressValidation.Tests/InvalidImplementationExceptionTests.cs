namespace Visus.AddressValidation.Tests;

using AwesomeAssertions;

internal sealed class InvalidImplementationExceptionTests
{
    [Test]
    public void DefaultConstructor_CreatesException()
    {
        InvalidImplementationException exception = new();

        exception.Message.Should().NotBeNull();
    }

    [Test]
    public void MessageAndInnerExceptionConstructor_SetsBoth()
    {
        InvalidOperationException inner = new("inner");
        InvalidImplementationException exception = new("outer", inner);

        exception.Message.Should().Be("outer");
        exception.InnerException.Should().BeSameAs(inner);
        exception.HResult.Should().Be(-2146233079);
    }

    [Test]
    public void MessageConstructor_SetsMessageAndHResult()
    {
        InvalidImplementationException exception = new("test message");

        exception.Message.Should().Be("test message");
        exception.HResult.Should().Be(-2146233079);
    }
}
