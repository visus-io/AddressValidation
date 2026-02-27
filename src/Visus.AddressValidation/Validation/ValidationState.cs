namespace Visus.AddressValidation.Validation;

using System.Globalization;

/// <summary>
///     Defines a validation state
/// </summary>
public readonly struct ValidationState : IEquatable<ValidationState>
{
    private ValidationState(ValidationSeverity severity, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Message = message;
        Severity = severity;
    }

    /// <summary>
    ///     Gets the message
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the severity
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    ///     Creates an error validation state with a given message (optionally associated with a property).
    /// </summary>
    /// <param name="message">The message associated with the validation state.</param>
    /// <param name="propertyName">The name of the property associated with the validation state.</param>
    /// <returns><see cref="ValidationState" /> instance.</returns>
    public static ValidationState CreateError(string message, string? propertyName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return !string.IsNullOrWhiteSpace(propertyName)
                   ? new ValidationState(ValidationSeverity.Error, $"{propertyName}: {message}")
                   : new ValidationState(ValidationSeverity.Error, message);
    }

    /// <summary>
    ///     Creates an error validation with a given message.
    /// </summary>
    /// <param name="messageFormat">
    ///     A
    ///     <see href="https://learn.microsoft.com/dotnet/standard/base-types/composite-formatting">composite format string</see>
    ///     .
    /// </param>
    /// <param name="args">An object array to contains zero or more objects to format.</param>
    /// <returns><see cref="ValidationState" /> instance.</returns>
    public static ValidationState CreateError(string messageFormat, params object[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageFormat);
        string message = string.Format(CultureInfo.InvariantCulture, messageFormat, args);
        return CreateError(message);
    }

    /// <summary>
    ///     Creates a warning validation state with a given message (optionally associated with a property).
    /// </summary>
    /// <param name="message">The message associated with the validation state.</param>
    /// <param name="propertyName">The name of the property associated with the validation state.</param>
    /// <returns><see cref="ValidationState" /> instance.</returns>
    public static ValidationState CreateWarning(string message, string? propertyName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return !string.IsNullOrWhiteSpace(propertyName)
                   ? new ValidationState(ValidationSeverity.Warning, $"{propertyName}: {message}")
                   : new ValidationState(ValidationSeverity.Warning, message);
    }

    /// <summary>
    ///     Creates a warning validation state with a given message.
    /// </summary>
    /// <param name="messageFormat">
    ///     A
    ///     <see href="https://learn.microsoft.com/dotnet/standard/base-types/composite-formatting">composite format string</see>
    ///     .
    /// </param>
    /// <param name="args">An object array to contains zero or more objects to format.</param>
    /// <returns><see cref="ValidationState" /> instance.</returns>
    public static ValidationState CreateWarning(string messageFormat, params object[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageFormat);
        string message = string.Format(CultureInfo.InvariantCulture, messageFormat, args);
        return CreateWarning(message);
    }

    /// <summary>
    ///     Indicates whether the values of the two specified <see cref="ValidationState" /> objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns><c>true</c> if <c>left</c> and <c>right</c> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ValidationState left, ValidationState right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Indicates whether the values of the two specified <see cref="ValidationState" /> objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns><c>true</c> if <c>left</c> and <c>right</c> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ValidationState left, ValidationState right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public bool Equals(ValidationState other)
    {
        return string.Equals(Message, other.Message, StringComparison.OrdinalIgnoreCase)
            && Severity == other.Severity;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ValidationState other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hashCode = new();

        hashCode.Add(Message, StringComparer.OrdinalIgnoreCase);
        hashCode.Add((int)Severity);

        return hashCode.ToHashCode();
    }

    /// <summary>
    ///     Returns the validation state message.
    /// </summary>
    /// <returns>The validation state message.</returns>
    public override string ToString()
    {
        return Message;
    }
}
