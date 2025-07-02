namespace Visus.AddressValidation.Integration.Google.Http;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Validation;
using Model;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed partial class ApiResponse : IApiResponse
{
    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; init; }

    [CustomResponseDataProperty]
    public Guid ResponseId { get; init; }

    public Response? Result { get; init; }

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

    internal sealed partial class Geocode
    {
        public Location Location { get; set; } = null!;

        [CustomResponseDataProperty("googlePlaceId")]
        public string PlaceId { get; set; } = null!;
    }

    internal sealed partial class Location
    {
        [CustomResponseDataProperty]
        public decimal Latitude { get; set; }

        [CustomResponseDataProperty]
        public decimal Longitude { get; set; }
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

    internal sealed partial class UspsData
    {
        [CustomResponseDataProperty]
        public string? AddressRecordType { get; set; }

        [CustomResponseDataProperty]
        public string? CarrierRoute { get; set; }

        [CustomResponseDataProperty]
        public string? CarrierRouteIndicator { get; set; }

        [CustomResponseDataProperty]
        public bool? CassProcessed { get; set; }

        [CustomResponseDataProperty]
        public string? County { get; set; }

        [CustomResponseDataProperty]
        public string? DeliveryPointCheckDigit { get; set; }

        [CustomResponseDataProperty]
        public string? DeliveryPointCode { get; set; }

        [CustomResponseDataProperty]
        public string? DpvCmra { get; set; }

        [CustomResponseDataProperty]
        public string? DpvConfirmation { get; set; }

        [CustomResponseDataProperty]
        public string? DpvDoorNotAccessible { get; set; }

        [CustomResponseDataProperty]
        public string? DpvDrop { get; set; }

        [CustomResponseDataProperty]
        public string? DpvEnhancedDeliveryCode { get; set; }

        [CustomResponseDataProperty]
        public string? DpvFootnote { get; set; }

        [CustomResponseDataProperty]
        public string? DpvNonDeliveryDays { get; set; }

        [CustomResponseDataProperty]
        public string? DpvNoSecureLocation { get; set; }

        [CustomResponseDataProperty]
        public string? DpvNoStat { get; set; }

        [CustomResponseDataProperty]
        public int? DpvNoStatReasonCode { get; set; }

        [CustomResponseDataProperty]
        public string? DpvPbsa { get; set; }

        [CustomResponseDataProperty]
        public string? DpvThrowback { get; set; }

        [CustomResponseDataProperty]
        public string? DpvVacant { get; set; }

        [CustomResponseDataProperty]
        public string? ElotFlag { get; set; }

        [CustomResponseDataProperty]
        public string? ElotNumber { get; set; }

        [CustomResponseDataProperty]
        public string? FipsCountyCode { get; set; }

        [CustomResponseDataProperty]
        public string? PostOfficeCity { get; set; }

        [CustomResponseDataProperty]
        public string? PostOfficeState { get; set; }
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
