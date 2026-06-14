namespace Visus.AddressValidation.Integration.Google.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using AddressValidation.Configuration;

/// <summary>
///     Configuration options for the Google Address Validation API service
///     account.
/// </summary>
public sealed class GoogleServiceOptions : AbstractServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:Google";

    /// <summary>
    ///     Initializes a new instance of <see cref="GoogleServiceOptions" />
    ///     with <see cref="AbstractServiceOptions.ClientEnvironment" /> defaulting
    ///     to <see cref="ClientEnvironment.PRODUCTION" />.
    /// </summary>
    public GoogleServiceOptions()
    {
        ClientEnvironment = ClientEnvironment.PRODUCTION;
    }

    /// <summary>
    ///     Gets the base URI of the Google OAuth 2.0 token endpoint, derived
    ///     from the current <see cref="AbstractServiceOptions.ClientEnvironment" /> value.
    /// </summary>
    public Uri AuthenticationUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT or ClientEnvironment.PRODUCTION => Constants.ProductionAuthenticationUri,
            ClientEnvironment.SANDBOX => AuthenticationUriOverride!,
            _ => Constants.ProductionAuthenticationUri,
        };

    /// <inheritdoc />
    public override Uri EndpointUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT or ClientEnvironment.PRODUCTION => Constants.ProductionEndpointUri,
            ClientEnvironment.SANDBOX => EndpointUriOverride!,
            _ => Constants.ProductionEndpointUri,
        };

    /// <summary>
    ///     Gets or sets a URI that overrides the default authentication endpoint
    ///     derived from <see cref="AbstractServiceOptions.ClientEnvironment" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property is <b>required</b> when
    ///         <see cref="AbstractServiceOptions.ClientEnvironment" /> is
    ///         <see cref="ClientEnvironment.SANDBOX" />; validation will fail
    ///         if it is <see langword="null" /> in that case.
    ///     </para>
    ///     <para>
    ///         For all other environments this property is optional and, when
    ///         set, has no effect — the authentication endpoint is always
    ///         resolved from <see cref="AbstractServiceOptions.ClientEnvironment" />.
    ///     </para>
    /// </remarks>
    public Uri? AuthenticationUriOverride { get; set; }

    /// <summary>
    ///     Gets or sets the RSA private key associated with the service account,
    ///     used to sign JWT tokens for authentication.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string PrivateKey { get; set; }

    /// <summary>
    ///     Gets or sets the Google Cloud project ID that hosts the Address
    ///     Validation API.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ProjectId { get; set; }

    /// <summary>
    ///     Gets or sets the email address of the Google service account used to
    ///     authenticate API requests.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ServiceAccountEmail { get; set; }

    /// <summary>
    ///     Performs cross-property validation on the options object.
    /// </summary>
    /// <param name="validationContext">
    ///     The context in which validation is performed.
    /// </param>
    /// <returns>
    ///     A collection of <see cref="ValidationResult" /> instances describing
    ///     any validation failures, or an empty collection if the options are
    ///     valid.
    /// </returns>
    /// <remarks>
    ///     Inherits the sandbox endpoint override check from
    ///     <see cref="AbstractServiceOptions.Validate" /> and additionally validates
    ///     that <see cref="AuthenticationUriOverride" /> is not
    ///     <see langword="null" /> when
    ///     <see cref="AbstractServiceOptions.ClientEnvironment" /> is
    ///     <see cref="ClientEnvironment.SANDBOX" />.
    /// </remarks>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach ( ValidationResult result in base.Validate(validationContext) )
        {
            yield return result;
        }

        if ( ClientEnvironment == ClientEnvironment.SANDBOX && AuthenticationUriOverride is null )
        {
            yield return new ValidationResult(
                $"{nameof(AuthenticationUriOverride)} must be set when {nameof(ClientEnvironment)} is {nameof(ClientEnvironment.SANDBOX)}.",
                [nameof(AuthenticationUriOverride),]);
        }
    }
}
