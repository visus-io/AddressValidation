namespace Visus.AddressValidation.Integration.FedEx.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration options for the FedEx address validation service.
/// </summary>
public sealed class FedExServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:FedEx";

    /// <summary>
    ///     Gets the base URI of the FedEx API endpoint, derived from the
    ///     current <see cref="ClientEnvironment" /> value.
    /// </summary>
    public Uri EndpointBaseUri =>
        EndpointOverrideUri ?? ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            _ => Constants.DevelopmentEndpointBaseUri,
        };

    /// <summary>
    ///     Gets or sets the FedEx account number used to authenticate API
    ///     requests.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string AccountNumber { get; set; }

    /// <summary>
    ///     Gets or sets the target client environment, which determines
    ///     whether requests are sent to the FedEx sandbox or production
    ///     endpoint. Defaults to
    ///     <see cref="AddressValidation.Abstractions.ClientEnvironment.DEVELOPMENT" />.
    /// </summary>
    public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.DEVELOPMENT;

    /// <summary>
    ///     Gets or sets the OAuth 2.0 client ID issued by FedEx for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ClientId { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 client secret issued by FedEx for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ClientSecret { get; set; }

    /// <summary>
    ///     Gets or sets an optional URI that overrides the default endpoint
    ///     derived from <see cref="ClientEnvironment" />. When set, this value
    ///     takes precedence over the environment-based endpoint resolution.
    ///     This is primarily useful for testing against a local mock server.
    /// </summary>
    public Uri? EndpointOverrideUri { get; set; }
}
