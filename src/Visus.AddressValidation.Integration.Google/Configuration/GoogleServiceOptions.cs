namespace Visus.AddressValidation.Integration.Google.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration options for the Google Address Validation API service
///     account.
/// </summary>
public sealed class GoogleServiceOptions : IValidatableObject
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:Google";

    /// <summary>
    ///     Gets the base URI of the Google OAuth 2.0 token endpoint, derived
    ///     from the current <see cref="ClientEnvironment" /> value.
    /// </summary>
    public Uri AuthenticationUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT or ClientEnvironment.PRODUCTION => Constants.ProductionAuthenticationUri,
            ClientEnvironment.SANDBOX => AuthenticationUriOverride!,
            _ => Constants.ProductionAuthenticationUri,
        };

    /// <summary>
    ///     Gets the base URI of the Google Address Validation API endpoint,
    ///     derived from the current <see cref="ClientEnvironment" /> value.
    /// </summary>
    public Uri EndpointUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT or ClientEnvironment.PRODUCTION => Constants.ProductionEndpointUri,
            ClientEnvironment.SANDBOX => EndpointUriOverride!,
            _ => Constants.ProductionEndpointUri,
        };

    /// <summary>
    ///     Gets or sets a URI that overrides the default authentication endpoint
    ///     derived from <see cref="ClientEnvironment" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property is <b>required</b> when
    ///         <see cref="ClientEnvironment" /> is
    ///         <see cref="AddressValidation.Abstractions.ClientEnvironment.SANDBOX" />; validation will fail
    ///         if it is <see langword="null" /> in that case.
    ///     </para>
    ///     <para>
    ///         For all other environments this property is optional and, when
    ///         set, has no effect — the authentication endpoint is always
    ///         resolved from <see cref="ClientEnvironment" />.
    ///     </para>
    /// </remarks>
    public Uri? AuthenticationUriOverride { get; set; }

    /// <summary>
    ///     Gets or sets the target client environment, which determines
    ///     whether requests are sent to the Google production endpoint or a
    ///     local sandbox mock server. Defaults to
    ///     <see cref="AddressValidation.Abstractions.ClientEnvironment.PRODUCTION" />.
    /// </summary>
    public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.PRODUCTION;

    /// <summary>
    ///     Gets or sets a URI that overrides the default endpoint derived from
    ///     <see cref="ClientEnvironment" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property is <b>required</b> when
    ///         <see cref="ClientEnvironment" /> is
    ///         <see cref="ClientEnvironment.SANDBOX" />; validation will fail
    ///         if it is <see langword="null" /> in that case.
    ///     </para>
    ///     <para>
    ///         For all other environments this property is optional and, when
    ///         set, has no effect — the endpoint is always resolved from
    ///         <see cref="ClientEnvironment" />.
    ///     </para>
    /// </remarks>
    public Uri? EndpointUriOverride { get; set; }

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
    ///     Validates that neither <see cref="AuthenticationUriOverride" /> nor
    ///     <see cref="EndpointUriOverride" /> is <see langword="null" /> when
    ///     <see cref="ClientEnvironment" /> is
    ///     <see cref="ClientEnvironment.SANDBOX" />, since the sandbox
    ///     environment requires explicit URIs to target a local mock server.
    /// </remarks>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ( ClientEnvironment != ClientEnvironment.SANDBOX )
        {
            yield break;
        }

        if ( AuthenticationUriOverride is null )
        {
            yield return new ValidationResult(
                $"{nameof(AuthenticationUriOverride)} must be set when {nameof(ClientEnvironment)} is {nameof(ClientEnvironment.SANDBOX)}.",
                [nameof(AuthenticationUriOverride),]);
        }

        if ( EndpointUriOverride is null )
        {
            yield return new ValidationResult(
                $"{nameof(EndpointUriOverride)} must be set when {nameof(ClientEnvironment)} is {nameof(ClientEnvironment.SANDBOX)}.",
                [nameof(EndpointUriOverride),]);
        }
    }
}
