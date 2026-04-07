namespace Visus.AddressValidation.Tests.Serialization.Json;

using System.Text.Json;
using AddressValidation.Serialization.Json;
using AwesomeAssertions;

internal sealed class JsonStringBooleanConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new JsonStringBooleanConverter(),
        },
    };

    [Test]
    [Arguments("\"\"")]
    [Arguments("\"   \"")]
    [Arguments("\"0\"")]
    [Arguments("\"F\"")]
    [Arguments("\"False\"")]
    [Arguments("\"false\"")]
    [Arguments("\"N\"")]
    [Arguments("\"NO\"")]
    public void Read_FalseStrings_ReturnsFalse(string json)
    {
        bool result = JsonSerializer.Deserialize<bool>(json, Options);

        result.Should().BeFalse();
    }

    [Test]
    [Arguments("true", true)]
    [Arguments("false", false)]
    public void Read_JsonBooleanTokens_ReturnsCorrectValue(string json, bool expected)
    {
        bool result = JsonSerializer.Deserialize<bool>(json, Options);

        result.Should().Be(expected);
    }

    [Test]
    [Arguments("\"1\"")]
    [Arguments("\"-1\"")]
    [Arguments("\"T\"")]
    [Arguments("\"True\"")]
    [Arguments("\"true\"")]
    [Arguments("\"Y\"")]
    [Arguments("\"YES\"")]
    public void Read_TrueStrings_ReturnsTrue(string json)
    {
        bool result = JsonSerializer.Deserialize<bool>(json, Options);

        result.Should().BeTrue();
    }

    [Test]
    public void Read_UnknownString_ReturnsDefaultFalse()
    {
        bool result = JsonSerializer.Deserialize<bool>("\"unknown\"", Options);

        result.Should().BeFalse();
    }

    [Test]
    [Arguments(true, "true")]
    [Arguments(false, "false")]
    public void Write_OutputsBooleanValue(bool value, string expected)
    {
        string json = JsonSerializer.Serialize(value, Options);

        json.Should().Be(expected);
    }
}
