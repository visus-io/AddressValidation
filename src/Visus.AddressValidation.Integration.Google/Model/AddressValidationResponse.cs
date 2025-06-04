namespace Visus.AddressValidation.Integration.Google.Model;

using System.Collections.ObjectModel;
using AddressValidation.Extensions;
using AddressValidation.Model;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
{
    public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
        : base(response, validationResult)
    {
        if ( response.Result is null )
        {
            return;
        }

        AddressLines = response.Result.Address.PostalAddress.AddressLines
                               .Select(s => s.ToUpperInvariant())
                               .ToHashSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = response.Result.Address.PostalAddress.Locality?.ToUpperInvariant();
        Country = response.Result.Address.PostalAddress.RegionCode;
        PostalCode = response.Result.Address.PostalAddress.PostalCode?.ToUpperInvariant();
        StateOrProvince = response.Result.Address.PostalAddress.AdministrativeArea?.ToUpperInvariant();

        if ( response.Result.Metadata is not null )
        {
            IsResidential = response.Result.Metadata.Residential;
        }

        Dictionary<string, object?> customResponseData = new(StringComparer.OrdinalIgnoreCase);

        customResponseData.Merge(response.GetCustomResponseData());
        customResponseData.Merge(response.Result.Geocode.GetCustomResponseData());
        customResponseData.Merge(response.Result.Geocode.Location.GetCustomResponseData());

        if ( response.Result.UspsData is not null )
        {
            customResponseData.Merge(response.Result.UspsData.GetCustomResponseData());
        }

        CustomResponseData = new ReadOnlyDictionary<string, object?>(customResponseData);
    }
}
