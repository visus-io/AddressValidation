namespace Visus.AddressValidation.Google.Http;

using System.Text.Json.Serialization;
using AddressValidation.Abstractions;
using Serialization.Json;

/// <inheritdoc />
[JsonConverter(typeof(AddressValidationRequestConverter))]
public sealed class GoogleAddressValidationRequest : AbstractAddressValidationRequest
{
	private readonly HashSet<CountryCode> _cassEnabledCountries =
	[
		CountryCode.US,
		CountryCode.PR
	];

	/// <summary>
	///     Gets if USPS CASS processing is supported
	/// </summary>
	/// <remarks>Currently, USPS CASS is only supported for <see cref="CountryCode.US" /> and <see cref="CountryCode.PR" />.</remarks>
	public bool EnableUspsCass => Country is not null && _cassEnabledCountries.Contains(Country.Value);
}
