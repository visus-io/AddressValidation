namespace Visus.AddressValidation.Tests.Serialization.Json;

using System.Text.Json;
using AddressValidation.Serialization.Json;
using AwesomeAssertions;

internal sealed class JsonStringDateOnlyConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new JsonStringDateOnlyConverter(),
        },
    };

    [Test]
    [Arguments("\"not-a-date\"")]
    [Arguments("\"\"")]
    public void Read_InvalidString_ReturnsNull(string json)
    {
        DateOnly? result = JsonSerializer.Deserialize<DateOnly?>(json, Options);

        result.Should().BeNull();
    }

    [Test]
    public void Read_NonStringToken_ThrowsJsonException()
    {
        Action act = () => JsonSerializer.Deserialize<DateOnly?>("123", Options);

        act.Should().Throw<JsonException>();
    }

    [Test]
    [Arguments("\"2024-01-15\"", 2024, 1, 15)]
    [Arguments("\"2023-12-31\"", 2023, 12, 31)]
    public void Read_ValidDateStrings_ParsesCorrectly(string json, int year, int month, int day)
    {
        DateOnly? result = JsonSerializer.Deserialize<DateOnly?>(json, Options);

        DateOnly expected = new(year, month, day);
        result.Should().Be(expected);
    }

    [Test]
    public void Write_Null_OutputsNullValue()
    {
        string json = JsonSerializer.Serialize<DateOnly?>(null, Options);

        json.Should().Be("null");
    }

    [Test]
    public void Write_ValidDate_OutputsIso8601()
    {
        DateOnly date = new(2024, 3, 15);

        string json = JsonSerializer.Serialize<DateOnly?>(date, Options);

        json.Should().Be("\"2024-03-15\"");
    }
}
