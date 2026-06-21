namespace Visus.AddressValidation.Integration.FedEx.Models;

using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Representation of a unified address validation request to FedEx.
/// </summary>
[UsedImplicitly]
public sealed class FedExAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <inheritdoc />
    public override string? NoPostalCodeFallback => "00000";

    /// <summary>
    ///     Gets or sets an optional client-defined reference identifier passed through to the FedEx API response.
    /// </summary>
    public string? ClientReferenceId { get; set; }

    /// <summary>
    ///     Gets or sets the Customer Transaction ID used to identify the transaction.
    /// </summary>
    public string? CustomerTransactionId { get; set; }
}
