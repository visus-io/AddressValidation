namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Validation;
using Model;

internal sealed partial class ApiResponse : IApiResponse
{
    [JsonIgnore]
    public ApiErrorResponse? ErrorResponse { get; set; }

    [JsonPropertyName("address")]
    public AddressResult? Result { get; set; }

    public AddressSuggestion? Suggestions { get; set; }

    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed partial class AddressResult
    {
        public string[] AddressLines { get; set; } = [];

        [CustomResponseDataProperty]
        public string? CarrierRoute { get; set; }

        public string? CityTown { get; set; }

        public CountryCode CountryCode { get; set; }

        [CustomResponseDataProperty]
        public string? DeliveryPoint { get; set; }

        public string? PostalCode { get; set; }

        public bool? Residential { get; set; }

        public string? StateProvince { get; set; }

        public StatusCode Status { get; set; }
    }

    internal sealed class AddressSuggestion
    {
        [JsonPropertyName("address")]
        public AddressResult[]? Addresses { get; set; }

        public string? Suggestion { get; set; }
    }
}
