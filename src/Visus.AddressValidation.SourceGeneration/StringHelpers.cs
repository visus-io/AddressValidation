namespace Visus.AddressValidation.SourceGeneration;

/// <summary>
///     Provides string utility methods used during source generation.
/// </summary>
internal static class StringHelpers
{
    /// <summary>
    ///     Converts the first character of <paramref name="name" /> to lower-case,
    ///     returning a camelCase representation of the identifier.
    /// </summary>
    /// <param name="name">The identifier to convert.</param>
    /// <returns>
    ///     <paramref name="name" /> with its first character lower-cased, or the original
    ///     value if it is <see langword="null" /> or empty.
    /// </returns>
    internal static string ToCamelCase(string name)
    {
        return string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
    }

    /// <summary>
    ///     Converts a fully qualified type name into a string that is safe to use as a
    ///     file name by replacing characters that are invalid in file systems.
    /// </summary>
    /// <param name="fullyQualifiedName">The fully qualified type name to convert.</param>
    /// <returns>
    ///     A sanitized string with <c>global::</c> removed and the characters
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
