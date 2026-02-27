namespace Visus.AddressValidation.Integration.FedEx.Model;

using Abstractions;
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

        ApiResponse.ResolvedAddress primaryAddress = response.Result.ResolvedAddresses[0];

        AddressLines = primaryAddress.StreetLinesToken
                                     .Select(s => s.ToUpperInvariant())
                                     .ToHashSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = primaryAddress.City;
        Country = primaryAddress.CountryCode;
        PostalCode = primaryAddress.PostalCode;
        StateOrProvince = primaryAddress.StateOrProvince;
        IsResidential = primaryAddress.Classification == AddressClassification.RESIDENTIAL;

        Dictionary<string, object?> customResponseData = new(StringComparer.OrdinalIgnoreCase);

        customResponseData.Merge(response.GetCustomResponseData());
        customResponseData.Merge(primaryAddress.GetCustomResponseData());
        customResponseData.Merge(primaryAddress.Attributes.GetCustomResponseData());

        CustomResponseData = customResponseData;
    }
}
