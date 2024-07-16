namespace Visus.AddressValidation.Google.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using Http;

/// <summary>
///     Converts an <see cref="GoogleAddressValidationRequest" /> object to and from json.
/// </summary>
public sealed class AddressValidationRequestConverter : JsonConverter<GoogleAddressValidationRequest?>
{
	private const string AddressLinesPropertyName = "addressLines";
	private const string AddressPropertyName = "address";
	private const string AdministrativeAreaPropertyName = "administrativeArea";
	private const string EnableUspsCassPropertyName = "enableUspsCass";
	private const string LocalityPropertyName = "locality";
	private const string PostalCodePropertyName = "postalCode";
	private const string RegionCodePropertyName = "regionCode";

	/// <inheritdoc />
	public override GoogleAddressValidationRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, GoogleAddressValidationRequest? value, JsonSerializerOptions options)
	{
		if ( value is null )
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();
		writer.WriteStartObject(AddressPropertyName);

		writer.WriteStartArray(AddressLinesPropertyName);

		foreach ( string addressLine in value.AddressLines )
		{
			writer.WriteStringValue(addressLine);
		}

		writer.WriteEndArray();

		if ( !string.IsNullOrWhiteSpace(value.StateOrProvince) )
		{
			writer.WriteString(AdministrativeAreaPropertyName, value.StateOrProvince);
		}

		if ( !string.IsNullOrWhiteSpace(value.CityOrTown) )
		{
			writer.WriteString(LocalityPropertyName, value.CityOrTown);
		}

		if ( !string.IsNullOrWhiteSpace(value.PostalCode) )
		{
			writer.WriteString(PostalCodePropertyName, value.PostalCode);
		}

		writer.WriteString(RegionCodePropertyName, value.Country!.Value.ToString());

		writer.WriteEndObject();

		writer.WriteBoolean(EnableUspsCassPropertyName, value.EnableUspsCass);

		writer.WriteEndObject();
	}
}
