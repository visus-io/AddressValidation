namespace Visus.AddressValidation.Serialization.Json;

using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///     Converts JSON strings representing boolean values to and from <see cref="bool" />.
/// </summary>
public sealed class JsonStringBooleanConverter : JsonConverter<bool>
{
    private static readonly FrozenSet<string> PossibleBooleanFalseStrings =
        new[]
        {
            "0",
            "F",
            "False",
            "N",
            "NO",
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> PossibleBooleanTrueStrings =
        new[]
        {
            "-1",
            "1",
            "T",
            "True",
            "Y",
            "YES",
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

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
