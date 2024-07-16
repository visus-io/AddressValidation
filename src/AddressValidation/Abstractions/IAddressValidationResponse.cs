namespace Visus.AddressValidation.Abstractions;

/// <summary>
///     Represents a uniformed address validation response.
/// </summary>
public interface IAddressValidationResponse
{
	/// <summary>
	///     Gets the address lines
	/// </summary>
	public IReadOnlyCollection<string> AddressLines { get; }

	/// <summary>
	///     Gets the city (town)
	/// </summary>
	public string? CityOrTown { get; }

	/// <summary>
	///     Gets the country code
	/// </summary>
	/// <remarks>Refer to <see cref="Country" /> for values.</remarks>
	public CountryCode Country { get; }

	/// <summary>
	///     Gets any errors returned during validation
	/// </summary>
	public IReadOnlyCollection<string> Errors { get; }

	/// <summary>
	///     Gets the zip (postal) code
	/// </summary>
	/// <remarks>Value may be omitted for countries that do not support the concept of a postal code.</remarks>
	public string? PostalCode { get; }

	/// <summary>
	///     Gets the state (province)
	/// </summary>
	/// <remarks>Value may be omitted for countries that are considered city-states.</remarks>
	public string? StateOrProvince { get; }

	/// <summary>
	///     Gets suggested addresses created during validation
	/// </summary>
	/// <remarks>Collection may be empty if no suggestions provided or validation service does not provide them.</remarks>
	public IReadOnlyCollection<IAddressValidationResponse> Suggestions { get; }

	/// <summary>
	///     Gets any warnings returned during validation
	/// </summary>
	public IReadOnlyCollection<string> Warnings { get; }
}
