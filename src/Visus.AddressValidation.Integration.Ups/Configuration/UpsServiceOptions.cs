namespace Visus.AddressValidation.Integration.Ups.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using AddressValidation.Configuration;

/// <summary>
///     Configuration options for the UPS address validation service.
/// </summary>
public sealed class UpsServiceOptions : AbstractServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:Ups";

    /// <inheritdoc />
    public override Uri EndpointUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointUri,
            ClientEnvironment.SANDBOX => EndpointUriOverride!,
            _ => Constants.DevelopmentEndpointUri,
        };

    /// <summary>
    ///     Gets or sets the UPS account number used to authenticate API
    ///     requests.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string AccountNumber { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 client ID issued by UPS for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ClientId { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 client secret issued by UPS for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ClientSecret { get; set; }
}
