namespace Visus.AddressValidation.Configuration;

using System.ComponentModel.DataAnnotations;
using Abstractions;

/// <summary>
///     Abstract base class for one provider's address validation service
///     options. It defines common environment, endpoint, and sandbox
///     validation behavior.
/// </summary>
public abstract class AbstractServiceOptions : IValidatableObject
{
    /// <summary>
    ///     Gets the base URI of the provider's API endpoint, derived from the
    ///     current <see cref="ClientEnvironment" /> value.
    /// </summary>
    public abstract Uri EndpointUri { get; }

    /// <summary>
    ///     Gets or sets the target client environment. It determines whether
    ///     requests go to the provider's sandbox or production endpoint.
    ///     Defaults to
    ///     <see cref="Abstractions.ClientEnvironment.DEVELOPMENT" />.
    /// </summary>
    public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.DEVELOPMENT;

    /// <summary>
    ///     Gets or sets a URI that overrides the default endpoint derived from
    ///     <see cref="ClientEnvironment" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property is <b>required</b> when
    ///         <see cref="ClientEnvironment" /> is
    ///         <see cref="ClientEnvironment.SANDBOX" />. Validation fails if
    ///         it is <see langword="null" /> in that case.
    ///     </para>
    ///     <para>
    ///         For all other environments, this property is optional. When
    ///         set, it has no effect — <see cref="ClientEnvironment" />
    ///         always determines the endpoint.
    ///     </para>
    /// </remarks>
    public Uri? EndpointUriOverride { get; set; }

    /// <summary>
    ///     Performs cross-property validation on the options object.
    /// </summary>
    /// <param name="validationContext">
    ///     The context in which validation is performed.
    /// </param>
    /// <returns>
    ///     A collection of <see cref="ValidationResult" /> instances that
    ///     describe validation failures. The collection is empty when the
    ///     options are valid.
    /// </returns>
    /// <remarks>
    ///     Validates that <see cref="EndpointUriOverride" /> is not
    ///     <see langword="null" /> when <see cref="ClientEnvironment" /> is
    ///     <see cref="ClientEnvironment.SANDBOX" />. The sandbox environment
    ///     needs an explicit endpoint to target a local mock server.
    /// </remarks>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ( ClientEnvironment == ClientEnvironment.SANDBOX && EndpointUriOverride is null )
        {
            yield return new ValidationResult(
                $"{nameof(EndpointUriOverride)} must be set when {nameof(ClientEnvironment)} is {nameof(ClientEnvironment.SANDBOX)}.",
                [nameof(EndpointUriOverride),]);
        }
    }
}
