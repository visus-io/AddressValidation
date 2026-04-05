namespace Visus.AddressValidation.Integration.FedEx.Http;

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Serialization.Json;
using AddressValidation.Validation;
using Model;

internal sealed class ApiResponse : IApiResponse, ICustomResponseData
{
    public string? CustomerTransactionId { get; set; }

    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; set; }

    [JsonPropertyName("output")]
    public Response? Result { get; set; }

    public Guid TransactionId { get; set; }

    public IReadOnlyDictionary<string, object?> GetCustomResponseData()
    {
        JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

        return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            { policy.ConvertName(nameof(CustomerTransactionId)), CustomerTransactionId },
            { policy.ConvertName(nameof(TransactionId)), TransactionId },
        });
    }

    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed class Attribute : ICustomResponseData
    {
        [JsonPropertyName(nameof(AddressPrecision))]
        public string? AddressPrecision { get; set; }

        [JsonPropertyName(nameof(AddressType))]
        public AddressType AddressType { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("ValidMultiUnit")]
        public bool ContainsMultipleUnits { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(CountrySupported))]
        public bool CountrySupported { get; set; }

        [JsonConverter(typeof(JsonStringDateOnlyConverter))]
        [JsonPropertyName(nameof(DataVintage))]
        public DateOnly? DataVintage { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(Inserted))]
        public bool Inserted { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(InvalidSuiteNumber))]
        public bool InvalidSuiteNumber { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(IsBaseAddressForMultiUnit))]
        public bool IsBaseAddressForMultiUnit { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("DPV")]
        public bool IsDeliveryPointValid { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("POBox")]
        public bool IsPostOfficeBox { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("POBoxOnlyZIP")]
        public bool IsPostOfficeBoxOnlyZip { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName("RRConversion")]
        public bool IsRuralRouteConversion { get; set; }

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

        [JsonPropertyName(nameof(MatchSource))]
        public string MatchSource { get; set; } = null!;

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(MultipleMatches))]
        public bool MultipleMatches { get; set; }

        [JsonPropertyName(nameof(ResolutionMethod))]
        public ResolutionMethod ResolutionMethod { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(Resolved))]
        public bool Resolved { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(SplitZip))]
        public bool SplitZip { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(SuiteRequiredButMissing))]
        public bool SuiteRequiredButMissing { get; set; }

        [JsonConverter(typeof(JsonStringBooleanConverter))]
        [JsonPropertyName(nameof(ValidlyFormed))]
        public bool ValidlyFormed { get; set; }

        public IReadOnlyDictionary<string, object?> GetCustomResponseData()
        {
            JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                { policy.ConvertName(nameof(AddressPrecision)), AddressPrecision },
                { policy.ConvertName(nameof(AddressType)), AddressType },
                { policy.ConvertName(nameof(ContainsMultipleUnits)), ContainsMultipleUnits },
                { policy.ConvertName(nameof(CountrySupported)), CountrySupported },
                { policy.ConvertName(nameof(DataVintage)), DataVintage },
                { policy.ConvertName(nameof(Inserted)), Inserted },
                { policy.ConvertName(nameof(IsBaseAddressForMultiUnit)), IsBaseAddressForMultiUnit },
                { policy.ConvertName(nameof(IsDeliveryPointValid)), IsDeliveryPointValid },
                { policy.ConvertName(nameof(IsPostOfficeBox)), IsPostOfficeBox },
                { policy.ConvertName(nameof(IsPostOfficeBoxOnlyZip)), IsPostOfficeBoxOnlyZip },
                { policy.ConvertName(nameof(IsRuralRouteConversion)), IsRuralRouteConversion },
                { policy.ConvertName(nameof(IsUniquePostalCode)), IsUniquePostalCode },
                { policy.ConvertName(nameof(MatchSource)), MatchSource },
                { policy.ConvertName(nameof(ResolutionMethod)), ResolutionMethod },
                { policy.ConvertName(nameof(Resolved)), Resolved },
                { policy.ConvertName(nameof(SplitZip)), SplitZip },
                { policy.ConvertName(nameof(ValidlyFormed)), ValidlyFormed },
            });
        }
    }

    internal sealed class CustomerMessage
    {
        public string Code { get; set; } = null!;

        public string Message { get; set; } = null!;
    }

    internal sealed class ResolvedAddress : ICustomResponseData
    {
        public Attribute Attributes { get; set; } = null!;

        public string City { get; set; } = null!;

        public AddressClassification Classification { get; set; }

        public CountryCode CountryCode { get; set; }

        public CustomerMessage[]? CustomerMessages { get; set; }

        public bool GeneralDelivery { get; set; }

        [JsonPropertyName("normalizedStatusNameDPV")]
        public bool NormalizedStatusNameDpv { get; set; }

        public string PostalCode { get; set; } = null!;

        [JsonPropertyName("resolutionMethodName")]
        public ResolutionMethod ResolutionMethod { get; set; }

        public bool RuralRouteHighwayContract { get; set; }

        public string StandardizedStatusNameMatchSource { get; set; } = null!;

        [JsonPropertyName("stateOrProvinceCode")]
        public string? StateOrProvince { get; set; }

        public string[] StreetLinesToken { get; set; } = [];

        public IReadOnlyDictionary<string, object?> GetCustomResponseData()
        {
            JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                { policy.ConvertName(nameof(GeneralDelivery)), GeneralDelivery },
                { policy.ConvertName(nameof(NormalizedStatusNameDpv)), NormalizedStatusNameDpv },
                { policy.ConvertName(nameof(RuralRouteHighwayContract)), RuralRouteHighwayContract },
                { policy.ConvertName(nameof(StandardizedStatusNameMatchSource)), StandardizedStatusNameMatchSource },
            });
        }
    }

    internal sealed class Response
    {
        public ResolvedAddress[] ResolvedAddresses { get; set; } = [];
    }
}
