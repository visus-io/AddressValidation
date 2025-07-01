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

    [JsonPropertyName("suggestions")]
    public AddressSuggestion? Suggestions { get; set; }

    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed partial class AddressResult
    {
        [JsonPropertyName("addressLines")]
        public string[] AddressLines { get; set; } = [];

        [CustomResponseDataProperty("carrierRoute")]
        [JsonPropertyName("carrierRoute")]
        public string? CarrierRoute { get; set; }

        [JsonPropertyName("cityTown")]
        public string? CityTown { get; set; }
        
        [JsonPropertyName("countryCode")]
        public CountryCode CountryCode { get; set; }

        [CustomResponseDataProperty("deliveryPoint")]
        [JsonPropertyName("deliveryPoint")]
        public string? DeliveryPoint { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("residential")]
        public bool? Residential { get; set; }

        [JsonPropertyName("stateProvince")]
        public string? StateProvince { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<StatusCode>))]
        [JsonPropertyName("status")]
        public StatusCode Status { get; set; }
    }

    internal sealed class AddressSuggestion
    {
        [JsonPropertyName("address")]
        public AddressResult[]? Addresses { get; set; }

        [JsonPropertyName("suggestion")]
        public string? Suggestion { get; set; }
    }
}
