namespace Visus.AddressValidation.Integration.Google.Http;

using System.Net;

internal sealed class ApiErrorResponse
{
    public ErrorResponse? Error { get; set; }

    internal sealed class ErrorResponse
    {
        public HttpStatusCode Code { get; set; }

        public string? Message { get; set; }
    }
}
