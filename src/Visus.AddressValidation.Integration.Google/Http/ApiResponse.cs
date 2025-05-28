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
	[JsonPropertyName("responseId")]
	public Guid ResponseId { get; init; }

	[JsonPropertyName("result")]
	public Response? Result { get; init; }

	public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
	{
		return new AddressValidationResponse(this, validationResult);
	}

	internal sealed class Address
	{
		[JsonPropertyName("addressComponents")]
		public AddressComponent[] AddressComponents { get; set; } = [];

		[JsonPropertyName("formattedAddress")]
		public string? FormattedAddress { get; set; }

		[JsonPropertyName("postalAddress")]
		public PostalAddress PostalAddress { get; set; } = null!;
	}

	internal sealed class AddressComponent
	{
		[JsonPropertyName("componentName")]
		public ComponentName? ComponentName { get; set; }

		[JsonPropertyName("componentType")]
		public string? ComponentType { get; set; }

		[JsonPropertyName("confirmationLevel")]
		[JsonConverter(typeof(JsonStringEnumConverter<ConfirmationLevel>))]
		public ConfirmationLevel ConfirmationLevel { get; set; }

		[JsonPropertyName("inferred")]
		public bool? Inferred { get; set; }
	}

	internal sealed class ComponentName
	{
		[JsonPropertyName("languageCode")]
		public string? LanguageCode { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}

	internal sealed partial class Geocode
	{
		[JsonPropertyName("location")]
		public Location Location { get; set; } = null!;

		[CustomResponseDataProperty("googlePlaceId")]
		[JsonPropertyName("placeId")]
		public string PlaceId { get; set; } = null!;
	}

	internal sealed partial class Location
	{
		[CustomResponseDataProperty]
		[JsonPropertyName("latitude")]
		public decimal Latitude { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("longitude")]
		public decimal Longitude { get; set; }
	}

	internal sealed class Metadata
	{
		[JsonPropertyName("business")]
		public bool Business { get; set; }

		[JsonPropertyName("residential")]
		public bool Residential { get; set; }
	}

	internal sealed class PostalAddress
	{
		[JsonPropertyName("addressLines")]
		public string[] AddressLines { get; set; } = [];

		[JsonPropertyName("administrativeArea")]
		public string? AdministrativeArea { get; set; }

		[JsonPropertyName("languageCode")]
		public string? LanguageCode { get; set; }

		[JsonPropertyName("locality")]
		public string? Locality { get; set; }

		[JsonPropertyName("postalCode")]
		public string? PostalCode { get; set; }

		[JsonPropertyName("regionCode")]
		[JsonConverter(typeof(JsonStringEnumConverter<CountryCode>))]
		public CountryCode RegionCode { get; set; }
	}

	internal sealed class Response
	{
		[JsonPropertyName("address")]
		public Address Address { get; set; } = null!;

		[JsonPropertyName("geocode")]
		public Geocode Geocode { get; set; } = null!;

		[JsonPropertyName("metadata")]
		public Metadata? Metadata { get; set; }

		[JsonPropertyName("uspsData")]
		public UspsData? UspsData { get; set; }

		[JsonPropertyName("verdict")]
		public Verdict Verdict { get; set; } = null!;
	}

	internal sealed partial class UspsData
	{
		[CustomResponseDataProperty]
		[JsonPropertyName("addressRecordType")]
		public string? AddressRecordType { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("carrierRoute")]
		public string? CarrierRoute { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("carrierRouteIndicator")]
		public string? CarrierRouteIndicator { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("cassProcessed")]
		public bool? CassProcessed { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("county")]
		public string? County { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("deliveryPointCheckDigit")]
		public string? DeliveryPointCheckDigit { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("deliveryPointCode")]
		public string? DeliveryPointCode { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvCmra")]
		public string? DpvCmra { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvConfirmation")]
		public string? DpvConfirmation { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvDoorNotAccessible")]
		public string? DpvDoorNotAccessible { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvDrop")]
		public string? DpvDrop { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvEnhancedDeliveryCode")]
		public string? DpvEnhancedDeliveryCode { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvFootnote")]
		public string? DpvFootnote { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvNonDeliveryDays")]
		public string? DpvNonDeliveryDays { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvNoSecureLocation")]
		public string? DpvNoSecureLocation { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvNoStat")]
		public string? DpvNoStat { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvNoStatReasonCode")]
		public int? DpvNoStatReasonCode { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvPbsa")]
		public string? DpvPbsa { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvThrowback")]
		public string? DpvThrowback { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("dpvVacant")]
		public string? DpvVacant { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("elotFlag")]
		public string? ElotFlag { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("elotNumber")]
		public string? ElotNumber { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("fipsCountyCode")]
		public string? FipsCountyCode { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("postOfficeCity")]
		public string? PostOfficeCity { get; set; }

		[CustomResponseDataProperty]
		[JsonPropertyName("postOfficeState")]
		public string? PostOfficeState { get; set; }
	}

	internal sealed class Verdict
	{
		[JsonPropertyName("addressComplete")]
		public bool AddressComplete { get; set; }

		[JsonPropertyName("geocodeGranularity")]
		[JsonConverter(typeof(JsonStringEnumConverter<Granularity>))]
		public Granularity GeocodeGranularity { get; set; }

		[JsonPropertyName("hasInferredComponents")]
		public bool HasInferredComponents { get; set; }

		[JsonPropertyName("hasReplacedComponents")]
		public bool HasReplacedComponents { get; set; }

		[JsonPropertyName("inputGranularity")]
		[JsonConverter(typeof(JsonStringEnumConverter<Granularity>))]
		public Granularity InputGranularity { get; set; }

		[JsonPropertyName("validationGranularity")]
		[JsonConverter(typeof(JsonStringEnumConverter<Granularity>))]
		public Granularity ValidationGranularity { get; set; }
	}
}
