namespace Visus.AddressValidation.Integration.PitneyBowes.Mappers;

using AddressValidation.Mappers;
using Contracts;
using Model;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
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
