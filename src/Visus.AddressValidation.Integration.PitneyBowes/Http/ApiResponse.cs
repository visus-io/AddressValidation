using System.Text.Json.Serialization;
using Visus.AddressValidation.Abstractions;
using Visus.AddressValidation.Http;
using Visus.AddressValidation.Integration.PitneyBowes.Abstractions;
using Visus.AddressValidation.Integration.PitneyBowes.Model;
using Visus.AddressValidation.Model;
using Visus.AddressValidation.Validation;

namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

internal sealed partial class ApiResponse : AbstractApiResponse
{
    [JsonIgnore] public ApiErrorResponse? ErrorResponse { get; set; }

    [JsonPropertyName("address")] public AddressResult? Result { get; set; }

    [JsonPropertyName("suggestions")] public AddressSuggestion? Suggestions { get; set; }

    public override IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new AddressValidationResponse(this, validationResult);
    }

    internal sealed partial class AddressResult
    {
        [JsonPropertyName("addressLines")] public string[] AddressLines { get; set; } = [];

        [CustomResponseDataProperty("carrierRoute")]
        [JsonPropertyName("carrierRoute")]
        public string? CarrierRoute { get; set; }

        [JsonPropertyName("cityTown")] public string? CityTown { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<CountryCode>))]
        [JsonPropertyName("countryCode")]
        public CountryCode CountryCode { get; set; }

        [CustomResponseDataProperty("deliveryPoint")]
        [JsonPropertyName("deliveryPoint")]
        public string? DeliveryPoint { get; set; }

        [JsonPropertyName("postalCode")] public string? PostalCode { get; set; }

        [JsonPropertyName("residential")] public bool? Residential { get; set; }

        [JsonPropertyName("stateProvince")] public string? StateProvince { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<StatusCode>))]
        [JsonPropertyName("status")]
        public StatusCode Status { get; set; }
    }

    internal sealed class AddressSuggestion
    {
        [JsonPropertyName("address")] public AddressResult[]? Addresses { get; set; }

        [JsonPropertyName("suggestion")] public string? Suggestion { get; set; }
    }
}
