namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Text.Json.Serialization;
using AddressValidation.Abstractions;

internal sealed class ApiRequest
{
    [JsonPropertyName("XAVRequest")]
    public required UpsXavRequest XavRequest { get; init; }

    internal sealed class UpsAddressKeyFormat
    {
        public required IReadOnlyList<string> AddressLine { get; init; }

        public required CountryCode CountryCode { get; init; }

        public string? PoliticalDivision1 { get; init; }

        public string? PoliticalDivision2 { get; init; }

        public string? PostcodeExtendedLow { get; init; }

        public required string PostcodePrimaryLow { get; init; }
    }

    internal sealed class UpsXavRequest
    {
        public required UpsAddressKeyFormat AddressKeyFormat { get; init; }
    }
}
