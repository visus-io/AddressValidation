namespace Visus.AddressValidation.Model;

using System.Text.Json.Serialization;
using Abstractions;

/// <summary>
///     Represents a uniformed address validation response.
/// </summary>
public interface IAddressValidationResponse
{
    /// <summary>
    ///     Gets the address lines
    /// </summary>
    [JsonPropertyName("addressLines")]
    IReadOnlySet<string> AddressLines { get; }

    /// <summary>
    ///     Gets the city (town)
    /// </summary>
    [JsonPropertyName("cityOrTown")]
    string? CityOrTown { get; }

    /// <summary>
    ///     Gets the country code
    /// </summary>
    /// <remarks>Refer to <see cref="Country" /> for values.</remarks>
    [JsonPropertyName("country")]
    CountryCode Country { get; }

    /// <summary>
    ///     Gets custom response data
    /// </summary>
    /// <remarks>Collection may be empty for services that do not provide additional response information.</remarks>
    [JsonPropertyName("customResponseData")]
    IReadOnlyDictionary<string, object?> CustomResponseData { get; }

    /// <summary>
    ///     Gets any errors returned during validation
    /// </summary>
    [JsonPropertyName("errors")]
    IReadOnlySet<string> Errors { get; }

    /// <summary>
    ///     Gets the residential indicator for the address
    /// </summary>
    /// <remarks>
    ///     Value may be <see langword="null" /> for services that do not
    ///     return an indicator.
    /// </remarks>
    [JsonPropertyName("isResidential")]
    bool? IsResidential { get; }

    /// <summary>
    ///     Gets the zip (postal) code
    /// </summary>
    /// <remarks>Value may be omitted for countries that do not support the concept of a postal code.</remarks>
    [JsonPropertyName("postalCode")]
    string? PostalCode { get; }

    /// <summary>
    ///     Gets the state (province)
    /// </summary>
    /// <remarks>Value may be omitted for countries that are considered city-states.</remarks>
    [JsonPropertyName("stateOrProvince")]
    string? StateOrProvince { get; }

    /// <summary>
    ///     Gets suggested addresses created during validation
    /// </summary>
    /// <remarks>Collection may be empty if no suggestions provided or validation service does not provide them.</remarks>
    [JsonPropertyName("suggestions")]
    IReadOnlyList<IAddressValidationResponse> Suggestions { get; }

    /// <summary>
    ///     Gets any warnings returned during validation
    /// </summary>
    [JsonPropertyName("warnings")]
    IReadOnlySet<string> Warnings { get; }
}
