namespace Visus.AddressValidation.Ups.Http;

using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Http.Abstractions;
using FluentValidation.Results;

internal sealed class ApiAddressValidationResponse : IApiAddressValidationResponse
{
	[JsonPropertyName("XAVResponse")]
	public XavResponse Result { get; init; } = null!;

	public IAddressValidationResponse ToAddressValidationResponse(ValidationResult? validationResult)
	{
		throw new NotImplementedException();
	}

	internal sealed class AddressClassification
	{
		public string? Code { get; set; }

		public string? Message { get; set; }
	}

	internal sealed class AddressKeyFormat
	{
		public string[] AddressLine { get; set; } = [];

		[JsonConverter(typeof(JsonStringEnumConverter))]
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
		public string? Code { get; set; }

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
