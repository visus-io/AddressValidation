namespace Visus.AddressValidation.Integration.Ups;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration constants for the UPS integration.
/// </summary>
public static class Constants
{
    /// <summary>
    ///     The UPS API development endpoint.
    /// </summary>
    public static readonly Uri DevelopmentEndpointUri = new("https://wwwcie.ups.com");

    /// <summary>
    ///     The UPS API production endpoint.
    /// </summary>
    public static readonly Uri ProductionEndpointUri = new("https://onlinetools.ups.com");

    /// <summary>
    ///     The countries the UPS Address Validation API supports.
    /// </summary>
    public static readonly FrozenSet<CountryCode> SupportedCountries =
    [
        CountryCode.US,
    ];
}
