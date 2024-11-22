namespace Visus.AddressValidation.Integration.Google.Http;

using System.Net;
using System.Text.Json.Serialization;

internal sealed class ApiErrorResponse
{
	[JsonPropertyName("error")]
	public ErrorResponse Error { get; set; } = null!;

	internal sealed class ErrorResponse
	{
		[JsonPropertyName("code")]
		public HttpStatusCode Code { get; set; }

		[JsonPropertyName("message")]
		public string? Message { get; set; }
	}
}
