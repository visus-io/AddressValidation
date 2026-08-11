namespace Visus.AddressValidation.Integration.Google.Models;

using AddressValidation.Abstractions;
using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Represents a unified address validation request sent to Google.
/// </summary>
[UsedImplicitly]
public sealed class GoogleAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets a value indicating whether USPS CASS processing is enabled for this request.
    /// </summary>
    /// <remarks>USPS CASS support is currently limited to <see cref="CountryCode.US" />.</remarks>
    public bool EnableUspsCass => Country == CountryCode.US;

    /// <summary>
    ///     Gets or sets the previous response ID.
    /// </summary>
    /// <remarks>
    ///     Set this value only when an address needs revalidation. Retrieve it from the previous response's
    ///     <see cref="IAddressValidationResponse.CustomResponseData" /> collection, under the key
    ///     <c>responseId</c>.
    /// </remarks>
    public Guid? PreviousResponseId { get; set; }
}
