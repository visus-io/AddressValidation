namespace Visus.AddressValidation.Integration.FedEx.Http;

internal sealed class ApiRequest
{
    public required IReadOnlyList<FedExAddressToValidate> AddressesToValidate { get; init; }

    internal sealed class FedExAddress
    {
        public string? City { get; init; }

        public required string CountryCode { get; init; }

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
