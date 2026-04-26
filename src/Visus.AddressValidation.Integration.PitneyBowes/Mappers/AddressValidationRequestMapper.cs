namespace Visus.AddressValidation.Integration.PitneyBowes.Mappers;

using AddressValidation.Mappers;
using Http;
using Model;

internal sealed class AddressValidationRequestMapper : IApiRequestMapper<PitneyBowesAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(PitneyBowesAddressValidationRequest request)
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
