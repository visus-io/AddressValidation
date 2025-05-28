namespace Visus.AddressValidation;

using Model;

/// <summary>
///     Specifies that the property be made available for use within a
///     <see cref="IAddressValidationResponse.CustomResponseData" /> collection.
/// </summary>
/// <remarks>
///     If <see cref="Name" /> has been defined, it will be used instead of the property name. Value will be converted to
///     camelCase.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CustomResponseDataPropertyAttribute : Attribute
{
	/// <summary>
	///     Initializes a new instance of <see cref="CustomResponseDataPropertyAttribute" />.
	/// </summary>
	public CustomResponseDataPropertyAttribute()
	{
	}

	/// <summary>
	///     Initializes a new instance of <see cref="CustomResponseDataPropertyAttribute" /> with the specified name.
	/// </summary>
	/// <param name="name">The name to use instead of the property name.</param>
	public CustomResponseDataPropertyAttribute(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Name = name;
	}

	/// <summary>
	/// </summary>
	public string? Name { get; }
}
