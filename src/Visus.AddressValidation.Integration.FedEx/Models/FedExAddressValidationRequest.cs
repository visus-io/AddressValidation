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
    /// <remarks>
    ///     When submitted via <see cref="AddressValidation.Services.IBatchAddressValidationService{TRequest}" />,
    ///     FedEx's API accepts only one transaction identifier for the entire batch call, so only the value set on
    ///     the first request in the batch is transmitted. Use <see cref="ClientReferenceId" /> to correlate
    ///     individual items within a batch.
    /// </remarks>
    public string? CustomerTransactionId { get; set; }
}
