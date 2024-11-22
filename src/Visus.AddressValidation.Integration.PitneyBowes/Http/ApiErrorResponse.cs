namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Text.Json.Serialization;

internal sealed class ApiErrorResponse
{
	[JsonPropertyName("errors")]
	public Error[] Errors { get; set; } = [];

	internal sealed class Error
	{
		[JsonPropertyName("additionalInfo")]
		public string? AdditionalInfo { get; set; }

		[JsonPropertyName("errorCode")]
		public string? ErrorCode { get; set; }

		[JsonPropertyName("errorDescription")]
		public string? ErrorDescription { get; set; }

		[JsonPropertyName("parameters")]
		public string[]? Parameters { get; set; }
	}
}
