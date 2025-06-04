namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Text.Json.Serialization;

internal sealed class ApiErrorResponse
{
    [JsonPropertyName("response")]
    public ErrorResponse Response { get; set; } = null!;

    internal sealed class Error
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    internal sealed class ErrorResponse
    {
        [JsonPropertyName("errors")]
        public Error[] Errors { get; set; } = [];
    }
}
