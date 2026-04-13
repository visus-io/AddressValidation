namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using AddressValidation.Abstractions;
using Http;

/// <summary>
///     Converts an <see cref="UpsAddressValidationRequest" /> object to and from JSON.
/// </summary>
public sealed class AddressValidationRequestConverter : JsonConverter<UpsAddressValidationRequest?>
{
    private const string AddressKeyFormatPropertyName = "AddressKeyFormat";

    private const string AddressLinePropertyName = "AddressLine";

    private const string CountryCodePropertyName = "CountryCode";

    private const string PoliticalDivision1PropertyName = "PoliticalDivision1";

    private const string PoliticalDivision2PropertyName = "PoliticalDivision2";

    private const string PostcodeExtendedLowPropertyName = "PostcodeExtendedLow";

    private const string PostcodePrimaryLowPropertyName = "PostcodePrimaryLow";

    private const string XavRequestPropertyName = "XAVRequest";

    /// <summary>
    ///     Reads a <see cref="UpsAddressValidationRequest" /> from JSON, mapping the UPS XAV fields
    ///     <c>AddressLine</c>, <c>PoliticalDivision2</c> (city), <c>PoliticalDivision1</c> (state/province),
    ///     <c>PostcodePrimaryLow</c>, <c>PostcodeExtendedLow</c>, and <c>CountryCode</c> to their request
    ///     properties. The two postcode parts are combined into a single <c>XXXXX-XXXX</c> postal code.
    /// </summary>
    public override UpsAddressValidationRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        UpsAddressValidationRequest request = new();
        string? postcodePrimary = null;
        string? postcodeExtended = null;

        while ( reader.Read() )
        {
            if ( reader.TokenType != JsonTokenType.PropertyName )
            {
                continue;
            }

            if ( reader.ValueTextEquals(AddressLinePropertyName) )
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

            if ( reader.ValueTextEquals(PoliticalDivision2PropertyName) )
            {
                reader.Read();
                request.CityOrTown = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(PoliticalDivision1PropertyName) )
            {
                reader.Read();
                request.StateOrProvince = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(PostcodePrimaryLowPropertyName) )
            {
                reader.Read();
                postcodePrimary = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(PostcodeExtendedLowPropertyName) )
            {
                reader.Read();
                postcodeExtended = reader.GetString();
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

        request.PostalCode = postcodeExtended is not null
                                 ? $"{postcodePrimary}-{postcodeExtended}"
                                 : postcodePrimary;

        return request;
    }

    /// <summary>
    ///     Writes a <see cref="UpsAddressValidationRequest" /> as JSON in UPS' XAV request format, producing
    ///     a nested <c>XAVRequest.AddressKeyFormat</c> structure with <c>AddressLine</c>,
    ///     <c>PoliticalDivision2</c> (city), <c>PoliticalDivision1</c> (state/province),
    ///     <c>PostcodePrimaryLow</c>, optional <c>PostcodeExtendedLow</c>, and <c>CountryCode</c>.
    ///     Writes <see langword="null" /> if the request is <see langword="null" />, or if
    ///     <see cref="UpsAddressValidationRequest.PostalCode" /> or
    ///     <see cref="UpsAddressValidationRequest.Country" /> is not set.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, UpsAddressValidationRequest? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if ( value is null || string.IsNullOrWhiteSpace(value.PostalCode) || value.Country is null )
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
