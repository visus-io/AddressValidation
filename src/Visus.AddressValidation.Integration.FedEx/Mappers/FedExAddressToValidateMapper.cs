namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using Contracts;
using Models;

internal static class FedExAddressToValidateMapper
{
    internal static ApiRequest.FedExAddressToValidate Map(FedExAddressValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ApiRequest.FedExAddressToValidate
        {
            Address = new ApiRequest.FedExAddress
            {
                StreetLines = [.. request.AddressLines,],
                City = request.CityOrTown,
                StateOrProvince = request.StateOrProvince,
                PostalCode = request.PostalCode,
                CountryCode = request.Country!.Value,
            },
            ClientReferenceId = request.ClientReferenceId,
        };
    }
}
