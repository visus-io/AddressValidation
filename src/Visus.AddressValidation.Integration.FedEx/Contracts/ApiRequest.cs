namespace Visus.AddressValidation.Integration.FedEx.Contracts;

using System.Text.Json.Serialization;
using AddressValidation.Abstractions;

internal sealed class ApiRequest
{
    public required IReadOnlyList<FedExAddressToValidate> AddressesToValidate { get; init; }

    [JsonIgnore]
    public string? CustomerTransactionId { get; init; }

    internal sealed class FedExAddress
    {
        public string? City { get; init; }

        public required CountryCode CountryCode { get; init; }

        public string? PostalCode { get; init; }

        public string? StateOrProvince { get; init; }

        public required IReadOnlyList<string> StreetLines { get; init; }
    }

    internal sealed class FedExAddressToValidate
    {
        public required FedExAddress Address { get; init; }

        public string? ClientReferenceId { get; init; }
    }
}
