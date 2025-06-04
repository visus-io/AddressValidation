namespace Visus.AddressValidation.Integration.Google.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using AddressValidation.Abstractions;
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
    private const string PreviousResponseId = "previousResponseId";
    private const string RegionCodePropertyName = "regionCode";

    /// <inheritdoc />
    public override GoogleAddressValidationRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        GoogleAddressValidationRequest request = new();

        while ( reader.Read() )
        {
            if ( reader.TokenType != JsonTokenType.PropertyName )
            {
                continue;
            }

            if ( reader.ValueTextEquals(AddressLinesPropertyName) )
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

            if ( reader.ValueTextEquals(AdministrativeAreaPropertyName) )
            {
                reader.Read();
                request.StateOrProvince = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(LocalityPropertyName) )
            {
                reader.Read();
                request.CityOrTown = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(PostalCodePropertyName) )
            {
                reader.Read();
                request.PostalCode = reader.GetString();
                continue;
            }

            if ( !reader.ValueTextEquals(RegionCodePropertyName) )
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

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GoogleAddressValidationRequest? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if ( value?.Country is null )
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

        writer.WriteString(RegionCodePropertyName, value.Country.Value.ToString());

        writer.WriteEndObject();

        if ( value.PreviousResponseId is not null && value.PreviousResponseId != Guid.Empty )
        {
            writer.WriteString(PreviousResponseId, value.PreviousResponseId.ToString());
        }

        writer.WriteBoolean(EnableUspsCassPropertyName, value.EnableUspsCass);

        writer.WriteEndObject();
    }
}
