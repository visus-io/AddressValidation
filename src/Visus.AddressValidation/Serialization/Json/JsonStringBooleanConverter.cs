namespace Visus.AddressValidation.Serialization.Json;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class JsonStringBooleanConverter : JsonConverter<bool>
{
    private static readonly HashSet<string> PossibleBooleanFalseStrings = new(StringComparer.OrdinalIgnoreCase)
    {
        "0",
        "F",
        "False",
        "N",
        "NO",
    };

    private static readonly HashSet<string> PossibleBooleanTrueStrings = new(StringComparer.OrdinalIgnoreCase)
    {
        "-1",
        "1",
        "T",
        "True",
        "Y",
        "YES",
    };

    /// <inheritdoc />
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.String => ToBoolOrDefault(reader.GetString()),
            _ => throw new JsonException(),
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteBooleanValue(value);
    }

    private static bool ToBoolOrDefault(string? value, bool defaultValue = false)
    {
        if ( string.IsNullOrWhiteSpace(value) )
        {
            return defaultValue;
        }

        if ( PossibleBooleanTrueStrings.Contains(value) )
        {
            return true;
        }

        if ( PossibleBooleanFalseStrings.Contains(value) )
        {
            return false;
        }

        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }
}
