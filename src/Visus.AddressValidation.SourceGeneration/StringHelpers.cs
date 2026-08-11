namespace Visus.AddressValidation.SourceGeneration;

/// <summary>
///     Provides string utility methods that the source generator uses.
/// </summary>
internal static class StringHelpers
{
    /// <summary>
    ///     Converts the first character of <paramref name="name" /> to lowercase. The result is a camelCase
    ///     version of <paramref name="name" />.
    /// </summary>
    /// <param name="name">The identifier to convert.</param>
    /// <returns>
    ///     <paramref name="name" /> with its first character lowercased. Returns the original value if
    ///     <paramref name="name" /> is <see langword="null" />, empty, or all white space.
    /// </returns>
    internal static string ToCamelCase(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
    }

    /// <summary>
    ///     Converts a fully qualified type name into a safe file name. It removes <c>global::</c> and replaces
    ///     <c>.</c>, <c>&lt;</c>, and <c>&gt;</c> with underscores.
    /// </summary>
    /// <param name="fullyQualifiedName">The fully qualified type name to convert.</param>
    /// <returns>
    ///     A sanitized string. It has <c>global::</c> removed and the characters
    ///     <c>.</c>, <c>&lt;</c>, and <c>&gt;</c> replaced with underscores.
    /// </returns>
    internal static string ToSafeFileName(string fullyQualifiedName)
    {
        return fullyQualifiedName
              .Replace("global::", string.Empty)
              .Replace('.', '_')
              .Replace('<', '_')
              .Replace('>', '_');
    }
}
