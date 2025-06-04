namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Validation;
using Model;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class ApiResponse : IApiResponse
{
    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; init; }

    [JsonPropertyName("XAVResponse")]
    public XavResponse? Result { get; init; }

    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed class AddressClassification
    {
        [JsonConverter(typeof(JsonStringEnumConverter<AddressClassificationCode>))]
        public AddressClassificationCode? Code { get; set; }

        public string? Message { get; set; }
    }

    internal sealed class AddressKeyFormat
    {
        public string[] AddressLine { get; set; } = [];

        [JsonConverter(typeof(JsonStringEnumConverter<CountryCode>))]
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
        [JsonConverter(typeof(JsonStringEnumConverter<ResponseStatusCode>))]
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
