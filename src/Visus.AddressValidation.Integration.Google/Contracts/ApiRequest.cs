namespace Visus.AddressValidation.Integration.Google.Contracts;

using AddressValidation.Abstractions;

internal sealed class ApiRequest
{
    public required GoogleAddress Address { get; init; }

    public bool EnableUspsCass { get; init; }

    public Guid? PreviousResponseId { get; init; }

    internal sealed class GoogleAddress
    {
        public required IReadOnlyList<string> AddressLines { get; init; }

        public string? AdministrativeArea { get; init; }

        public string? Locality { get; init; }

        public string? PostalCode { get; init; }

        public required CountryCode RegionCode { get; init; }
    }
}
