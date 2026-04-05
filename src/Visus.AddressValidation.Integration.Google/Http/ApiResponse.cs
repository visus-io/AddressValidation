namespace Visus.AddressValidation.Integration.Google.Http;

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Validation;
using Model;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class ApiResponse : IApiResponse, ICustomResponseData
{
    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; init; }

    public Guid ResponseId { get; init; }

    public Response? Result { get; init; }

    public IReadOnlyDictionary<string, object?> GetCustomResponseData()
    {
        return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            { "responseId", ResponseId },
        });
    }

    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed class Address
    {
        public AddressComponent[] AddressComponents { get; set; } = [];

        public string? FormattedAddress { get; set; }

        public PostalAddress PostalAddress { get; set; } = null!;
    }

    internal sealed class AddressComponent
    {
        public ComponentName? ComponentName { get; set; }

        public string? ComponentType { get; set; }

        public ConfirmationLevel ConfirmationLevel { get; set; }

        public bool? Inferred { get; set; }
    }

    internal sealed class ComponentName
    {
        public string? LanguageCode { get; set; }

        public string? Text { get; set; }
    }

    internal sealed class Geocode : ICustomResponseData
    {
        public Location Location { get; set; } = null!;

        public string PlaceId { get; set; } = null!;

        public IReadOnlyDictionary<string, object?> GetCustomResponseData()
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                { "googlePlaceId", PlaceId },
            });
        }
    }

    internal sealed class Location : ICustomResponseData
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public IReadOnlyDictionary<string, object?> GetCustomResponseData()
        {
            JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                { policy.ConvertName(nameof(Latitude)), Latitude },
                { policy.ConvertName(nameof(Longitude)), Longitude },
            });
        }
    }

    internal sealed class Metadata
    {
        public bool Business { get; set; }

        public bool Residential { get; set; }
    }

    internal sealed class PostalAddress
    {
        public string[] AddressLines { get; set; } = [];

        public string? AdministrativeArea { get; set; }

        public string? LanguageCode { get; set; }

        public string? Locality { get; set; }

        public string? PostalCode { get; set; }

        public CountryCode RegionCode { get; set; }
    }

    internal sealed class Response
    {
        public Address Address { get; set; } = null!;

        public Geocode Geocode { get; set; } = null!;

        public Metadata? Metadata { get; set; }

        public UspsData? UspsData { get; set; }

        public Verdict Verdict { get; set; } = null!;
    }

    internal sealed class UspsData : ICustomResponseData
    {
        public string? AddressRecordType { get; set; }

        public string? CarrierRoute { get; set; }

        public string? CarrierRouteIndicator { get; set; }

        public bool? CassProcessed { get; set; }

        public string? County { get; set; }

        public string? DeliveryPointCheckDigit { get; set; }

        public string? DeliveryPointCode { get; set; }

        public string? DpvCmra { get; set; }

        public string? DpvConfirmation { get; set; }

        public string? DpvDoorNotAccessible { get; set; }

        public string? DpvDrop { get; set; }

        public string? DpvEnhancedDeliveryCode { get; set; }

        public string? DpvFootnote { get; set; }

        public string? DpvNonDeliveryDays { get; set; }

        public string? DpvNoSecureLocation { get; set; }

        public string? DpvNoStat { get; set; }

        public int? DpvNoStatReasonCode { get; set; }

        public string? DpvPbsa { get; set; }

        public string? DpvThrowback { get; set; }

        public string? DpvVacant { get; set; }

        public string? ElotFlag { get; set; }

        public string? ElotNumber { get; set; }

        public string? FipsCountyCode { get; set; }

        public string? PostOfficeCity { get; set; }

        public string? PostOfficeState { get; set; }

        public IReadOnlyDictionary<string, object?> GetCustomResponseData()
        {
            JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                { policy.ConvertName(nameof(AddressRecordType)), AddressRecordType },
                { policy.ConvertName(nameof(CarrierRoute)), CarrierRoute },
                { policy.ConvertName(nameof(CarrierRouteIndicator)), CarrierRouteIndicator },
                { policy.ConvertName(nameof(CassProcessed)), CassProcessed },
                { policy.ConvertName(nameof(County)), County },
                { policy.ConvertName(nameof(DeliveryPointCheckDigit)), DeliveryPointCheckDigit },
                { policy.ConvertName(nameof(DeliveryPointCode)), DeliveryPointCode },
                { policy.ConvertName(nameof(DpvCmra)), DpvCmra },
                { policy.ConvertName(nameof(DpvConfirmation)), DpvConfirmation },
                { policy.ConvertName(nameof(DpvDoorNotAccessible)), DpvDoorNotAccessible },
                { policy.ConvertName(nameof(DpvDrop)), DpvDrop },
                { policy.ConvertName(nameof(DpvEnhancedDeliveryCode)), DpvEnhancedDeliveryCode },
                { policy.ConvertName(nameof(DpvFootnote)), DpvFootnote },
                { policy.ConvertName(nameof(DpvNonDeliveryDays)), DpvNonDeliveryDays },
                { policy.ConvertName(nameof(DpvNoSecureLocation)), DpvNoSecureLocation },
                { policy.ConvertName(nameof(DpvNoStat)), DpvNoStat },
                { policy.ConvertName(nameof(DpvNoStatReasonCode)), DpvNoStatReasonCode },
                { policy.ConvertName(nameof(DpvPbsa)), DpvPbsa },
                { policy.ConvertName(nameof(DpvThrowback)), DpvThrowback },
                { policy.ConvertName(nameof(DpvVacant)), DpvVacant },
                { policy.ConvertName(nameof(ElotFlag)), ElotFlag },
                { policy.ConvertName(nameof(ElotNumber)), ElotNumber },
                { policy.ConvertName(nameof(FipsCountyCode)), FipsCountyCode },
                { policy.ConvertName(nameof(PostOfficeCity)), PostOfficeCity },
                { policy.ConvertName(nameof(PostOfficeState)), PostOfficeState },
            });
        }
    }

    internal sealed class Verdict
    {
        public bool AddressComplete { get; set; }

        public Granularity GeocodeGranularity { get; set; }

        public bool HasInferredComponents { get; set; }

        public bool HasReplacedComponents { get; set; }

        public Granularity InputGranularity { get; set; }

        public Granularity ValidationGranularity { get; set; }
    }
}
