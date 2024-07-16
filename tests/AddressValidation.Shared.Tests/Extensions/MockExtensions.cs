namespace Visus.AddressValidation.Shared.Tests.Extensions;

using Microsoft.Extensions.Logging;
using Moq;

public static class MockExtensions
{
	public static Mock<ILogger<T>> VerifyLoggingCall<T>(this Mock<ILogger<T>> logger, LogLevel logLevel)
	{
		return VerifyLoggingCall(logger, logLevel, null, Times.Once());
	}

	public static Mock<ILogger<T>> VerifyLoggingCall<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, string? message)
	{
		return VerifyLoggingCall(logger, logLevel, message, Times.Once());
	}

	public static Mock<ILogger<T>> VerifyLoggingCall<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, string? message, Times times)
	{
		Func<object, Type, bool> state = (_, _) => true;
		if ( !string.IsNullOrWhiteSpace(message) )
		{
			state = (o, _) => string.Compare(o.ToString(), message, StringComparison.OrdinalIgnoreCase) == 0;
		}

		logger.Verify(v => v.Log(
								 It.Is<LogLevel>(l => l == LogLevel.Error),
								 It.IsAny<EventId>(),
								 It.Is<It.IsAnyType>((o, t) => state(o, t)),
								 It.IsAny<Exception>(),
								 It.Is<Func<It.IsAnyType, Exception?, string>>((_, _) => true)), times);

		return logger;
	}

	public static Mock<ILogger<T>> VerifyLoggingCall<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times)
	{
		return VerifyLoggingCall(logger, logLevel, null, times);
	}
}
