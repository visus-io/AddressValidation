namespace Visus.AddressValidation.Validation;

using Resources;

/// <summary>
///     Formats a provider-supplied code and message into a <see cref="ValidationState" />.
/// </summary>
public static class ApiMessageFormatter
{
    /// <summary>
    ///     Creates an error state from a provider error code and message.
    /// </summary>
    /// <param name="code">
    ///     The provider's error code. Can be <see langword="null" />, empty, or consist only of whitespace
    ///     when the provider omits one.
    /// </param>
    /// <param name="message">
    ///     The provider's error message. Can be <see langword="null" />, empty, or consist only of whitespace when the
    ///     provider omits one.
    /// </param>
    /// <returns>A <see cref="ValidationState" /> instance.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="code" /> and <paramref name="message" /> are both
    ///     <see langword="null" />, empty, or whitespace.
    /// </exception>
    public static ValidationState CreateError(string? code, string? message)
    {
        if ( !string.IsNullOrWhiteSpace(code) )
        {
            return string.IsNullOrWhiteSpace(message)
                       ? ValidationState.CreateError(code)
                       : ValidationState.CreateError($"{code}: {message}");
        }

        #pragma warning disable MA0015 // Neither parameter alone caused this failure; omit paramName instead of misattributing it to message.
        return string.IsNullOrWhiteSpace(message)
                   ? throw new ArgumentException(Resources.Validation_ApiMessageFormatter_CodeOrMessageRequired)
                   : ValidationState.CreateError(message);
        #pragma warning restore MA0015
    }

    /// <summary>
    ///     Creates a warning state from a provider code and message.
    /// </summary>
    /// <param name="code">
    ///     The provider's code. Can be <see langword="null" />, empty, or consist only of whitespace when the
    ///     provider omits one.
    /// </param>
    /// <param name="message">
    ///     The provider's message. Can be <see langword="null" />, empty, or consist only of whitespace
    ///     when the provider omits one.
    /// </param>
    /// <returns>A <see cref="ValidationState" /> instance.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="code" /> and <paramref name="message" /> are both
    ///     <see langword="null" />, empty, or whitespace.
    /// </exception>
    public static ValidationState CreateWarning(string? code, string? message)
    {
        if ( !string.IsNullOrWhiteSpace(code) )
        {
            return string.IsNullOrWhiteSpace(message)
                       ? ValidationState.CreateWarning(code)
                       : ValidationState.CreateWarning($"{code}: {message}");
        }

        #pragma warning disable MA0015 // Neither parameter alone caused this failure; omit paramName instead of misattributing it to message.
        return string.IsNullOrWhiteSpace(message)
                   ? throw new ArgumentException(Resources.Validation_ApiMessageFormatter_CodeOrMessageRequired)
                   : ValidationState.CreateWarning(message);
        #pragma warning restore MA0015
    }
}
