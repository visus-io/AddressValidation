namespace Visus.AddressValidation.Models;

using Abstractions;

/// <summary>
///     Represents a unified address validation response.
/// </summary>
public interface IAddressValidationResponse
{
    /// <summary>
    ///     Gets the validated address lines returned by the provider.
    /// </summary>
    [JsonPropertyName("addressLines")]
    IReadOnlySet<string> AddressLines { get; }

    /// <summary>
    ///     Gets the validated city or town name returned by the provider.
    /// </summary>
    [JsonPropertyName("cityOrTown")]
    string? CityOrTown { get; }

    /// <summary>
    ///     Gets the country code.
    /// </summary>
    /// <remarks>Refer to <see cref="CountryCode" /> for accepted values.</remarks>
    [JsonPropertyName("country")]
    CountryCode Country { get; }

    /// <summary>
    ///     Gets the custom response data.
    /// </summary>
    /// <remarks>The collection is empty for providers that offer no additional response information.</remarks>
    [JsonPropertyName("customResponseData")]
    IReadOnlyDictionary<string, object?> CustomResponseData { get; }

    /// <summary>
    ///     Gets the set of error messages from request or response validation. The set is empty when validation succeeds.
    /// </summary>
    [JsonPropertyName("errors")]
    IReadOnlySet<string> Errors { get; }

    /// <summary>
    ///     Gets the residential indicator for the address.
    /// </summary>
    /// <remarks>
    ///     Value may be <see langword="null" /> for providers that do not
    ///     return an indicator.
    /// </remarks>
    [JsonPropertyName("isResidential")]
    bool? IsResidential { get; }

    /// <summary>
    ///     Gets the postal code.
    /// </summary>
    /// <remarks>Value may be omitted for countries that do not support the concept of a postal code.</remarks>
    [JsonPropertyName("postalCode")]
    string? PostalCode { get; }

    /// <summary>
    ///     Gets the state or province.
    /// </summary>
    /// <remarks>Value may be omitted for countries that are considered city-states.</remarks>
    [JsonPropertyName("stateOrProvince")]
    string? StateOrProvince { get; }

    /// <summary>
    ///     Gets the suggested addresses created during validation.
    /// </summary>
    /// <remarks>The collection is empty when the provider offers no suggestions.</remarks>
    [JsonPropertyName("suggestions")]
    IReadOnlyList<IAddressValidationResponse> Suggestions { get; }

    /// <summary>
    ///     Gets the set of warning messages from request or response validation. The set is empty when no warnings exist.
    /// </summary>
    [JsonPropertyName("warnings")]
    IReadOnlySet<string> Warnings { get; }
}
