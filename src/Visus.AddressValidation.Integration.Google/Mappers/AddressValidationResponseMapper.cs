namespace Visus.AddressValidation.Integration.Google.Mappers;

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using AddressValidation.Extensions;
using AddressValidation.Mappers;
using AddressValidation.Model;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationResponseMapper : IApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, IValidationResult? validationResult = null)
    {
        if ( response.Result is null )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        Dictionary<string, object?> customResponseData = new(StringComparer.OrdinalIgnoreCase);

        customResponseData.Merge(response.GetCustomResponseData());
        customResponseData.Merge(response.Result.Geocode.GetCustomResponseData());
        customResponseData.Merge(response.Result.Geocode.Location.GetCustomResponseData());

        if ( response.Result.UspsData is not null )
        {
            customResponseData.Merge(response.Result.UspsData.GetCustomResponseData());
        }

        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = response.Result.Address.PostalAddress.AddressLines
                                   .ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            CityOrTown = response.Result.Address.PostalAddress.Locality,
            Country = response.Result.Address.PostalAddress.RegionCode,
            PostalCode = response.Result.Address.PostalAddress.PostalCode,
            StateOrProvince = response.Result.Address.PostalAddress.AdministrativeArea,
            IsResidential = response.Result.Metadata?.Residential,
            CustomResponseData = new ReadOnlyDictionary<string, object?>(customResponseData),
        };
    }
}
