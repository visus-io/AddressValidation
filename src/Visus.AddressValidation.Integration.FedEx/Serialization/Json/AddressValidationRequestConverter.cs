namespace Visus.AddressValidation.Integration.FedEx.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using AddressValidation.Abstractions;
using Http;

/// <summary>
///     Converts an <see cref="FedExAddressValidationRequest" /> object to and from JSON.
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

    /// <summary>
    ///     Reads a <see cref="FedExAddressValidationRequest" /> from JSON, parsing the nested
    ///     <c>addressesToValidate[].address</c> structure and mapping <c>streetLines</c>, <c>city</c>,
    ///     <c>stateOrProvince</c>, <c>postalCode</c>, and <c>countryCode</c> to their request properties.
    /// </summary>
    public override FedExAddressValidationRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        FedExAddressValidationRequest request = new();

        while ( reader.Read() )
        {
            if ( reader.TokenType != JsonTokenType.PropertyName )
            {
                continue;
            }

            if ( reader.ValueTextEquals(StreetLinesPropertyName) )
            {
                reader.Read();

                if ( reader.TokenType != JsonTokenType.StartArray )
                {
                    continue;
                }

                while ( reader.Read() && reader.TokenType != JsonTokenType.EndArray )
                {
                    if ( reader.TokenType != JsonTokenType.String )
                    {
                        continue;
                    }

                    string? line = reader.GetString();

                    if ( !string.IsNullOrWhiteSpace(line) )
                    {
                        request.AddressLines.Add(line);
                    }
                }

                continue;
            }

            if ( reader.ValueTextEquals(CityPropertyName) )
            {
                reader.Read();
                request.CityOrTown = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(StateOrProvincePropertyName) )
            {
                reader.Read();
                request.StateOrProvince = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(PostalCodePropertyName) )
            {
                reader.Read();
                request.PostalCode = reader.GetString();
                continue;
            }

            if ( !reader.ValueTextEquals(CountryCodePropertyName) )
            {
                continue;
            }

            reader.Read();

            if ( Enum.TryParse(reader.GetString(), out CountryCode countryCode) )
            {
                request.Country = countryCode;
            }
        }

        return request;
    }

    /// <summary>
    ///     Writes a <see cref="FedExAddressValidationRequest" /> as JSON in FedEx's address validation request
    ///     format, producing a nested <c>addressesToValidate[].address</c> structure with <c>streetLines</c>,
    ///     optional <c>city</c>, <c>stateOrProvince</c>, <c>postalCode</c>, and <c>countryCode</c>.
    ///     Writes <see langword="null" /> if <see cref="FedExAddressValidationRequest.Country" /> is not set.
    /// </summary>
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
