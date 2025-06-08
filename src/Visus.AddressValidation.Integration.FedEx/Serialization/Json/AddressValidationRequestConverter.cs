namespace Visus.AddressValidation.Integration.FedEx.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using Http;

/// <summary>
///     Converts an <see cref="FedExAddressValidationRequest" /> object to and from json.
/// </summary>
public sealed class AddressValidationRequestConverter : JsonConverter<FedExAddressValidationRequest?>
{
    private const string AddressesToValidatePropertyName = "addressesToValidate";
    private const string AddressPropertyName = "address";
    private const string CityPropertyName = "city";
    private const string CountryCodePropertyName = "countryCode";
    private const string PostalCodePropertyName = "postalCode";
    private const string StateOrProvincePropertyName = "stateOrProvince";
    private const string StreetLinesPropertyName = "streetLines";

    /// <inheritdoc />
    public override FedExAddressValidationRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FedExAddressValidationRequest? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if ( value?.Country is null )
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteStartArray(AddressesToValidatePropertyName);
        writer.WriteStartObject();
        writer.WriteStartObject(AddressPropertyName);
        
        writer.WriteStartArray(StreetLinesPropertyName);
        foreach ( string addressLine in value.AddressLines )
        {
            writer.WriteStringValue(addressLine);
        }
        writer.WriteEndArray();

        if ( !string.IsNullOrWhiteSpace(value.CityOrTown) )
        {
            writer.WriteString(CityPropertyName, value.CityOrTown);
        }

        if ( !string.IsNullOrWhiteSpace(value.StateOrProvince) )
        {
            writer.WriteString(StateOrProvincePropertyName, value.StateOrProvince);
        }

        if ( !string.IsNullOrWhiteSpace(value.PostalCode) )
        {
            writer.WriteString(PostalCodePropertyName, value.PostalCode);
        }
        
        writer.WriteString(CountryCodePropertyName, value.Country.Value.ToString());
        
        writer.WriteEndObject();
        writer.WriteEndObject();
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
