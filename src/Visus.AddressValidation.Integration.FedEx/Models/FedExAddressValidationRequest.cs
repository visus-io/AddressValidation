namespace Visus.AddressValidation.Integration.FedEx.Models;

using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Representation of a uniformed address validation request to FedEx.
/// </summary>
[UsedImplicitly]
public sealed class FedExAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <inheritdoc />
    public override string? NoPostalCodeFallback => "00000";

    /// <summary>
    ///     Gets or sets the Client Reference ID
    /// </summary>
    public string? ClientReferenceId { get; set; }

    /// <summary>
    ///     Gets or sets the Customer Transaction ID used to identify the transaction.
    /// </summary>
    public string? CustomerTransactionId { get; set; }
}
