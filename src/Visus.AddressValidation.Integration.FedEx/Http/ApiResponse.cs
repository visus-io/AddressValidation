namespace Visus.AddressValidation.Integration.FedEx.Http;

using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Serialization.Json;
using AddressValidation.Validation;
using Model;

internal sealed partial class ApiResponse : IApiResponse
{
    [CustomResponseDataProperty]
    public string? CustomerTransactionId { get; set; }

    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; set; }

    [JsonPropertyName("output")]
    public Response? Result { get; set; }

    [CustomResponseDataProperty]
    public Guid TransactionId { get; set; }

    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed partial class Attribute
    {
        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringEnumConverter<AddressPrecision>))]
        [JsonPropertyName(nameof(AddressPrecision))]
        public AddressPrecision AddressPrecision { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringEnumConverter<AddressType>))]
        [JsonPropertyName(nameof(AddressType))]
        public AddressType AddressType { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("ValidMultiUnit")]
        public bool ContainsMultipleUnits { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(CountrySupported))]
        public bool CountrySupported { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringDateOnlyConverter))]
        [JsonPropertyName(nameof(DataVintage))]
        public DateOnly? DataVintage { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(Inserted))]
        public bool Inserted { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(InvalidSuiteNumber))]
        public bool InvalidSuiteNumber { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(IsBaseAddressForMultiUnit))]
        public bool IsBaseAddressForMultiUnit { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("DPV")]
        public bool IsDeliveryPointValid { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("POBox")]
        public bool IsPostOfficeBox { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("POBoxOnlyZIP")]
        public bool IsPostOfficeBoxOnlyZip { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("RRConversion")]
        public bool IsRuralRouteConversion { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("UniqueZIP")]
        public bool IsUniquePostalCode { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("ZIP11Match")]
        public bool IsValidDeliveryPointCode { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("ZIP4Match")]
        public bool IsValidPostalCode { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("StreetAddress")]
        public bool IsValidStreetAddress { get; set; }

        [CustomResponseDataProperty]
        [JsonPropertyName(nameof(MatchSource))]
        public string MatchSource { get; set; } = null!;

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(MultipleMatches))]
        public bool MultipleMatches { get; set; }

        [CustomResponseDataProperty]
        [JsonPropertyName(nameof(ResolutionMethod))]
        public ResolutionMethod ResolutionMethod { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(Resolved))]
        public bool Resolved { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(SplitZip))]
        public bool SplitZip { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(SuiteRequiredButMissing))]
        public bool SuiteRequiredButMissing { get; set; }

        [CustomResponseDataProperty]
        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(ValidlyFormed))]
        public bool ValidlyFormed { get; set; }
    }

    internal sealed class CustomerMessage
    {
        public string Code { get; set; } = null!;

        public string Message { get; set; } = null!;
    }

    internal sealed partial class ResolvedAddress
    {
        public Attribute Attributes { get; set; } = null!;

        public string City { get; set; } = null!;
        
        public AddressClassification Classification { get; set; }

        public CountryCode CountryCode { get; set; }

        public CustomerMessage[]? CustomerMessages { get; set; }

        [CustomResponseDataProperty]
        public bool GeneralDelivery { get; set; }

        [CustomResponseDataProperty]
        [JsonPropertyName("normalizedStatusNameDPV")]
        public bool NormalizedStatusNameDpv { get; set; }

        public string PostalCode { get; set; } = null!;

        [CustomResponseDataProperty]
        public ResolutionMethod ResolutionMethod { get; set; }

        [CustomResponseDataProperty]
        public bool RuralRouteHighwayContract { get; set; }

        [CustomResponseDataProperty]
        public string StandardizedStatusNameMatchSource { get; set; } = null!;

        [JsonPropertyName("stateOrProvinceCode")]
        public string? StateOrProvince { get; set; }

        public string[] StreetLinesToken { get; set; } = [];
    }

    internal sealed class Response
    {
        public ResolvedAddress[] ResolvedAddresses { get; set; } = [];
    }
}
