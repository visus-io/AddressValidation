namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using Abstractions;
using AddressValidation.Extensions;
using AddressValidation.Mappers;
using AddressValidation.Models;
using AddressValidation.Validation;
using Contracts;
using Models;

internal sealed class AddressValidationResponseMapper : IApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, IValidationResult? validationResult = null)
    {
        ArgumentNullException.ThrowIfNull(response);

        if ( response.Result is null || response.Result.ResolvedAddresses.Length == 0 )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        ApiResponse.ResolvedAddress primary = response.Result.ResolvedAddresses[0];

        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = primary.StreetLinesToken
                                  .ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            CityOrTown = primary.City,
            Country = primary.CountryCode,
            PostalCode = primary.PostalCode,
            StateOrProvince = primary.StateOrProvince,
            IsResidential = primary.Classification == AddressClassification.RESIDENTIAL,
            CustomResponseData = BuildCustomResponseData(response, primary),
        };
    }

    private static ReadOnlyDictionary<string, object?> BuildCustomResponseData(
        ApiResponse response, ApiResponse.ResolvedAddress address)
    {
        Dictionary<string, object?> data = new(StringComparer.OrdinalIgnoreCase);

        data.Merge(response.GetCustomResponseData());
        data.Merge(address.GetCustomResponseData());
        data.Merge(address.Attributes.GetCustomResponseData());

        return data.AsReadOnly();
    }
}
