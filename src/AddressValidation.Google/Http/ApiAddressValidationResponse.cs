namespace Visus.AddressValidation.Google.Http;

using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http.Abstractions;
using FluentValidation.Results;
using Model;

internal sealed class ApiAddressValidationResponse : IApiAddressValidationResponse
{
	[JsonPropertyName("responseId")]
	public string ResponseId { get; init; } = null!;

	[JsonPropertyName("result")]
	public Response Result { get; init; } = null!;

	public IAddressValidationResponse ToAddressValidationResponse(ValidationResult? validationResult)
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
		[JsonConverter(typeof(JsonStringEnumConverter))]
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
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public CountryCode RegionCode { get; set; }
	}

	internal sealed class Response
	{
		[JsonPropertyName("address")]
		public Address Address { get; set; } = null!;

		[JsonPropertyName("verdict")]
		public Verdict Verdict { get; set; } = null!;
	}

	internal sealed class Verdict
	{
		[JsonPropertyName("addressComplete")]
		public bool AddressComplete { get; set; }

		[JsonPropertyName("geocodeGranularity")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Granularity GeocodeGranularity { get; set; }

		[JsonPropertyName("hasInferredComponents")]
		public bool HasInferredComponents { get; set; }

		[JsonPropertyName("inputGranularity")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Granularity InputGranularity { get; set; }

		[JsonPropertyName("validationGranularity")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Granularity ValidationGranularity { get; set; }
	}
}
