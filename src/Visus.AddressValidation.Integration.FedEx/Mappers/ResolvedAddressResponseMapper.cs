namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using System.Collections.ObjectModel;
using Abstractions;
using AddressValidation.Extensions;
using AddressValidation.Models;
using AddressValidation.Validation;
using Contracts;
using Models;

internal static class ResolvedAddressResponseMapper
{
    internal static IAddressValidationResponse Map(ApiResponse response, ApiResponse.ResolvedAddress address, IValidationResult? validationResult)
    {
        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = address.StreetLinesToken
                                  .ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            CityOrTown = address.City,
            Country = address.CountryCode,
            PostalCode = address.PostalCode,
            StateOrProvince = address.StateOrProvince,
            IsResidential = address.Classification == AddressClassification.RESIDENTIAL,
            CustomResponseData = BuildCustomResponseData(response, address),
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
