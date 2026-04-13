namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using AddressValidation.Abstractions;
using Http;

/// <summary>
///     Converts an <see cref="PitneyBowesAddressValidationRequest" /> object to and from JSON.
/// </summary>
public sealed class AddressValidationRequestConverter : JsonConverter<PitneyBowesAddressValidationRequest?>
{
    private const string AddressLinesPropertyName = "addressLines";

    private const string AddressPropertyName = "address";

    private const string CityTownPropertyName = "cityTown";

    private const string CountryCodePropertyName = "countryCode";

    private const string PostalCodePropertyName = "postalCode";

    private const string StateProvincePropertyName = "stateProvince";

    /// <summary>
    ///     Reads a <see cref="PitneyBowesAddressValidationRequest" /> from JSON, mapping <c>addressLines</c>,
    ///     <c>cityTown</c>, <c>stateProvince</c>, <c>postalCode</c>, and <c>countryCode</c> to their request
    ///     properties. The presence of an <c>address</c> wrapper object sets
    ///     <see cref="PitneyBowesAddressValidationRequest.IncludeSuggestions" /> to <see langword="true" />.
    /// </summary>
    public override PitneyBowesAddressValidationRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        PitneyBowesAddressValidationRequest request = new();

        while ( reader.Read() )
        {
            if ( reader.TokenType != JsonTokenType.PropertyName )
            {
                continue;
            }

            if ( reader.ValueTextEquals(AddressPropertyName) )
            {
                request.IncludeSuggestions = true;
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

            if ( reader.ValueTextEquals(CityTownPropertyName) )
            {
                reader.Read();
                request.CityOrTown = reader.GetString();
                continue;
            }

            if ( reader.ValueTextEquals(StateProvincePropertyName) )
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
    ///     Writes a <see cref="PitneyBowesAddressValidationRequest" /> as JSON in Pitney Bowes' address
    ///     validation request format, with <c>addressLines</c>, <c>cityTown</c>, <c>stateProvince</c>,
    ///     <c>postalCode</c>, and <c>countryCode</c>. When
    ///     <see cref="PitneyBowesAddressValidationRequest.IncludeSuggestions" /> is <see langword="true" />,
    ///     the address fields are wrapped in an <c>address</c> object.
    ///     Writes <see langword="null" /> if the request is <see langword="null" />, or if
    ///     <see cref="PitneyBowesAddressValidationRequest.PostalCode" /> or
    ///     <see cref="PitneyBowesAddressValidationRequest.Country" /> is not set.
    /// </summary>
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
