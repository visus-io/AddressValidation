namespace Visus.AddressValidation.SourceGeneration;

/// <summary>
///     Holds metadata about a property discovered during source generation.
/// </summary>
/// <param name="ContainingType">Metadata about the type that declares this property.</param>
/// <param name="Key">
///     The key used to look up the property value, typically the value supplied via
///     <c>CustomResponseDataPropertyAttribute</c>.
/// </param>
/// <param name="Name">The name of the property as declared in source code.</param>
/// <param name="TypeName">The fully qualified type name of the property.</param>
internal sealed record PropertyInfo(
    ContainingTypeInfo ContainingType,
    string Key,
    string Name,
    string TypeName)
{
    /// <summary>
    ///     Determines whether this instance is equal to another <see cref="PropertyInfo" />,
    ///     using case-insensitive comparisons for all string fields.
    /// </summary>
    /// <param name="other">The other <see cref="PropertyInfo" /> to compare against.</param>
    /// <returns>
    ///     <see langword="true" /> if all fields are equal (case-insensitive);
    ///     otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(PropertyInfo? other)
    {
        return other is not null
            && string.Equals(ContainingType.FullName, other.ContainingType.FullName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase)
            && string.Equals(TypeName, other.TypeName, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = ( hash * 31 ) + StringComparer.OrdinalIgnoreCase.GetHashCode(ContainingType.FullName);
            hash = ( hash * 31 ) + StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            hash = ( hash * 31 ) + StringComparer.OrdinalIgnoreCase.GetHashCode(Key);
            hash = ( hash * 31 ) + StringComparer.OrdinalIgnoreCase.GetHashCode(TypeName);

            return hash;
        }
    }
}
