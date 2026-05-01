namespace Visus.AddressValidation.Integration.Ups.Contracts;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class ApiResponse
{
    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; init; }

    [JsonPropertyName("XAVResponse")]
    public XavResponse? Result { get; init; }

    internal sealed class AddressClassification
    {
        public AddressClassificationCode? Code { get; set; }

        public string? Message { get; set; }
    }

    internal sealed class AddressKeyFormat
    {
        public string[] AddressLine { get; set; } = [];

        public CountryCode CountryCode { get; set; }

        public string? PoliticalDivision1 { get; set; }

        public string? PoliticalDivision2 { get; set; }

        public string? PostcodeExtendedLow { get; set; }

        public string? PostcodePrimaryLow { get; set; }

        public string? Region { get; set; }
    }

    internal sealed class Candidate
    {
        public AddressClassification AddressClassification { get; set; } = null!;

        public AddressKeyFormat AddressKeyFormat { get; set; } = null!;
    }

    internal sealed class Response
    {
        public ResponseStatus ResponseStatus { get; set; } = null!;
    }

    internal sealed class ResponseStatus
    {
        public ResponseStatusCode? Code { get; set; }

        public string? Message { get; set; }
    }

    internal sealed class XavResponse
    {
        public AddressClassification AddressClassification { get; set; } = null!;

        [JsonPropertyName("Candidate")]
        public Candidate[] Candidates { get; set; } = [];

        public Response Response { get; set; } = null!;

        public string? ValidAddressIndicator { get; set; }
    }
}
