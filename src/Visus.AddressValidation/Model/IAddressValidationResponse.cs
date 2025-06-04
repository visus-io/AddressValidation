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
    public IReadOnlySet<string> AddressLines { get; }

    /// <summary>
    ///     Gets the city (town)
    /// </summary>
    [JsonPropertyName("cityOrTown")]
    public string? CityOrTown { get; }

    /// <summary>
    ///     Gets the country code
    /// </summary>
    /// <remarks>Refer to <see cref="Country" /> for values.</remarks>
    [JsonConverter(typeof(JsonStringEnumConverter<CountryCode>))]
    [JsonPropertyName("country")]
    public CountryCode Country { get; }

    /// <summary>
    ///     Gets custom response data
    /// </summary>
    /// <remarks>Collection may be empty for services that do not provide additional response information.</remarks>
    [JsonPropertyName("customResponseData")]
    public IReadOnlyDictionary<string, object?> CustomResponseData { get; }

    /// <summary>
    ///     Gets any errors returned during validation
    /// </summary>
    [JsonPropertyName("errors")]
    public IReadOnlySet<string> Errors { get; }

    /// <summary>
    ///     Gets the residential indicator for the address
    /// </summary>
    /// <remarks>
    ///     Value may be <see langword="null" /> for services that do not
    ///     return an indicator.
    /// </remarks>
    [JsonPropertyName("isResidential")]
    public bool? IsResidential { get; }

    /// <summary>
    ///     Gets the zip (postal) code
    /// </summary>
    /// <remarks>Value may be omitted for countries that do not support the concept of a postal code.</remarks>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; }

    /// <summary>
    ///     Gets the state (province)
    /// </summary>
    /// <remarks>Value may be omitted for countries that are considered city-states.</remarks>
    [JsonPropertyName("stateOrProvince")]
    public string? StateOrProvince { get; }

    /// <summary>
    ///     Gets suggested addresses created during validation
    /// </summary>
    /// <remarks>Collection may be empty if no suggestions provided or validation service does not provide them.</remarks>
    [JsonPropertyName("suggestions")]
    public IReadOnlyList<IAddressValidationResponse> Suggestions { get; }

    /// <summary>
    ///     Gets any warnings returned during validation
    /// </summary>
    [JsonPropertyName("warnings")]
    public IReadOnlySet<string> Warnings { get; }
}
