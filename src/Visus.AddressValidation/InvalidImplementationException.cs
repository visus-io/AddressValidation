namespace Visus.AddressValidation;

/// <summary>
///     The exception that is thrown when a class does not implement a required base class.
/// </summary>
public class InvalidImplementationException : Exception
{
	/// <summary>
	///     Initializes a new instance of the <see cref="InvalidImplementationException" /> class.
	/// </summary>
	public InvalidImplementationException()
	{
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="InvalidImplementationException" /> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public InvalidImplementationException(string message)
		: base(message)
	{
		HResult = -2146233079;
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="InvalidImplementationException" /> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">
	///     The exception that is the cause of the current exception. If the
	///     <paramref name="innerException" /> parameter is not a <see langword="null" /> reference,
	///     the current exception is raised in a <see langword="catch" /> block that handles the inner exception.
	/// </param>
	public InvalidImplementationException(string message, Exception innerException)
		: base(message, innerException)
	{
		HResult = -2146233079;
	}
}
