namespace Visus.AddressValidation.Ups.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;
using Http;

/// <summary>
///     Converts an <see cref="UpsAddressValidationRequest" /> object to and from json.
/// </summary>
public class AddressValidationRequestConverter : JsonConverter<UpsAddressValidationRequest?>
{
	private const string AddressKeyFormatPropertyName = "AddressKeyFormat";

	private const string AddressLinePropertyName = "AddressLine";

	private const string CountryCodePropertyName = "CountryCode";

	private const string PoliticalDivision1PropertyName = "PoliticalDivision1";

	private const string PoliticalDivision2PropertyName = "PoliticalDivision2";

	private const string PostcodeExtendedLowPropertyName = "PostcodeExtendedLow";

	private const string PostcodePrimaryLowPropertyName = "PostcodePrimaryLow";

	private const string XavRequestPropertyName = "XAVRequest";

	/// <inheritdoc />
	public override UpsAddressValidationRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, UpsAddressValidationRequest? value, JsonSerializerOptions options)
	{
		if ( value is null || string.IsNullOrWhiteSpace(value.PostalCode) )
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();
		writer.WriteStartObject(XavRequestPropertyName);
		writer.WriteStartObject(AddressKeyFormatPropertyName);

		writer.WriteStartArray(AddressLinePropertyName);
		foreach ( string addressLine in value.AddressLines )
		{
			writer.WriteStringValue(addressLine);
		}

		writer.WriteEndArray();

		writer.WriteString(PoliticalDivision2PropertyName, value.CityOrTown);
		writer.WriteString(PoliticalDivision1PropertyName, value.StateOrProvince);
		
		string[] values = value.PostalCode!.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		writer.WriteString(PostcodePrimaryLowPropertyName, values[0]);

		if ( values.Length == 2 )
		{
			writer.WriteString(PostcodeExtendedLowPropertyName, values[1]);
		}

		writer.WriteString(CountryCodePropertyName, value.Country!.Value.ToString());

		writer.WriteEndObject();
		writer.WriteEndObject();
		writer.WriteEndObject();
	}
}
