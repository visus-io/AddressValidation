namespace Visus.AddressValidation.Validation;

/// <summary>
///     Formats a provider-supplied code and message into a <see cref="ValidationState" />.
/// </summary>
public static class ApiMessageFormatter
{
    /// <summary>
    ///     Creates an error state from a provider error code and message.
    /// </summary>
    /// <param name="code">The provider's error code. Can be <see langword="null" /> or empty when the provider omits one.</param>
    /// <param name="message">The provider's error message. Can be <see langword="null" /> or empty when the provider omits one.</param>
    /// <returns>A <see cref="ValidationState" /> instance.</returns>
    public static ValidationState CreateError(string? code, string? message)
    {
        if ( string.IsNullOrWhiteSpace(code) )
        {
            return ValidationState.CreateError(message ?? string.Empty);
        }

        return string.IsNullOrWhiteSpace(message)
                    ? ValidationState.CreateError(code)
                    : ValidationState.CreateError($"{code}: {message}");
    }

    /// <summary>
    ///     Creates a warning state from a provider code and message.
    /// </summary>
    /// <param name="code">The provider's code. Can be <see langword="null" /> or empty when the provider omits one.</param>
    /// <param name="message">The provider's message. Can be <see langword="null" /> or empty when the provider omits one.</param>
    /// <returns>A <see cref="ValidationState" /> instance.</returns>
    public static ValidationState CreateWarning(string? code, string? message)
    {
        if ( string.IsNullOrWhiteSpace(code) )
        {
            return ValidationState.CreateWarning(message ?? string.Empty);
        }

        return string.IsNullOrWhiteSpace(message)
                    ? ValidationState.CreateWarning(code)
                    : ValidationState.CreateWarning($"{code}: {message}");
    }
}
