namespace Visus.AddressValidation.Integration.PitneyBowes.Model;

using System.Collections.Frozen;
using AddressValidation.Abstractions;
using Contracts;
using Models;

internal sealed class AddressSuggestionValidationResponse : AbstractAddressValidationResponse
{
    public AddressSuggestionValidationResponse(ApiResponse.AddressResult addressResult, CountryCode countryCode, string? postalCode)
    {
        AddressLines = addressResult.AddressLines
                                    .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = addressResult.CityTown;
        Country = countryCode;
        PostalCode = postalCode;
        StateOrProvince = addressResult.StateProvince;
        IsResidential = addressResult.Residential;
    }
}
