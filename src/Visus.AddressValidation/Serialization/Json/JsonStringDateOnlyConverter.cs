namespace Visus.AddressValidation.Serialization.Json;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class JsonStringDateOnlyConverter : JsonConverter<DateOnly?>
{
    /// <inheritdoc />
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if ( reader.TokenType is not JsonTokenType.String )
        {
            throw new JsonException();
        }

        string? valueAsString = reader.GetString();

        return !DateOnly.TryParse(valueAsString, CultureInfo.InvariantCulture, out DateOnly date) ? null : date;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if ( value is null )
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}
