namespace Visus.AddressValidation.Integration.PitneyBowes.Mappers;

using Http;
using Model;

internal static class AddressValidationRequestMapper
{
    internal static ApiRequest ToApiRequest(this PitneyBowesAddressValidationRequest request)
    {
        return new ApiRequest
        {
            AddressLines = [..request.AddressLines,],
            CityTown = request.CityOrTown,
            CountryCode = request.Country!.Value,
            PostalCode = request.PostalCode!,
            IncludeSuggestions = request.IncludeSuggestions,
        };
    }
}
