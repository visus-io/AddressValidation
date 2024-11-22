namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using Http;

/// <summary>
///     Converts an <see cref="PitneyBowesAddressValidationRequest" /> object to and from json.
/// </summary>
public sealed class AddressValidationRequestConverter : JsonConverter<PitneyBowesAddressValidationRequest?>
{
	private const string AddressLinesPropertyName = "addressLines";

	private const string AddressPropertyName = "address";

	private const string CityTownPropertyName = "cityTown";

	private const string CountryCodePropertyName = "countryCode";

	private const string PostalCodePropertyName = "postalCode";

	private const string StateProvincePropertyName = "stateProvince";

	/// <inheritdoc />
	public override PitneyBowesAddressValidationRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, PitneyBowesAddressValidationRequest? value, JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(writer);

		if ( value is null || string.IsNullOrWhiteSpace(value.PostalCode) || value.Country is null )
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();

		if ( value.IncludeSuggestions )
		{
			writer.WriteStartObject(AddressPropertyName);
		}

		writer.WriteStartArray(AddressLinesPropertyName);
		foreach ( string addressLine in value.AddressLines )
		{
			writer.WriteStringValue(addressLine);
		}

		writer.WriteEndArray();

		writer.WriteString(CityTownPropertyName, value.CityOrTown);
		writer.WriteString(StateProvincePropertyName, value.StateOrProvince);
		writer.WriteString(PostalCodePropertyName, value.PostalCode);
		writer.WriteString(CountryCodePropertyName, value.Country.Value.ToString());

		if ( value.IncludeSuggestions )
		{
			writer.WriteEndObject();
		}

		writer.WriteEndObject();
	}
}
