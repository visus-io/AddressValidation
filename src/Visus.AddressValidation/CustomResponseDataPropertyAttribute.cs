namespace Visus.AddressValidation;

/// <summary>
///     Marks a property as part of the custom response data for an address
///     validation result, optionally overriding the name used when the property
///     is serialized or mapped.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CustomResponseDataPropertyAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CustomResponseDataPropertyAttribute" /> using the name of
    ///     the decorated property.
    /// </summary>
    public CustomResponseDataPropertyAttribute()
    {
    }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CustomResponseDataPropertyAttribute" /> with an explicit
    ///     name.
    /// </summary>
    /// <param name="name">
    ///     The name to use for the property instead of its declared name.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="name" /> is <see langword="null" />,
    ///     empty, or consists only of white-space characters.
    /// </exception>
    public CustomResponseDataPropertyAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    /// <summary>
    ///     Gets the explicit name to use for the property, or
    ///     <see langword="null" /> if the property's declared name should be
    ///     used.
    /// </summary>
    public string? Name { get; }
}
