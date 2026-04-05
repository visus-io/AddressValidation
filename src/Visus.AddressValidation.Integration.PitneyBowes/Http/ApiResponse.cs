namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Validation;
using Model;

internal sealed class ApiResponse : IApiResponse
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

    internal sealed class AddressResult : ICustomResponseData
    {
        public string[] AddressLines { get; set; } = [];

        public string? CarrierRoute { get; set; }

        public string? CityTown { get; set; }

        public CountryCode CountryCode { get; set; }

        public string? DeliveryPoint { get; set; }

        public string? PostalCode { get; set; }

        public bool? Residential { get; set; }

        public string? StateProvince { get; set; }

        public StatusCode Status { get; set; }

        public IReadOnlyDictionary<string, object?> GetCustomResponseData()
        {
            JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                { policy.ConvertName(nameof(CarrierRoute)), CarrierRoute },
                { policy.ConvertName(nameof(DeliveryPoint)), DeliveryPoint },
            });
        }
    }

    internal sealed class AddressSuggestion
    {
        [JsonPropertyName("address")]
        public AddressResult[]? Addresses { get; set; }

        public string? Suggestion { get; set; }
    }
}
