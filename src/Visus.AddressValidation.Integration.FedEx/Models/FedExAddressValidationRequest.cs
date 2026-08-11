namespace Visus.AddressValidation.Integration.FedEx.Models;

using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Represents a unified address validation request sent to FedEx.
/// </summary>
[UsedImplicitly]
public sealed class FedExAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <inheritdoc />
    public override string? NoPostalCodeFallback => "00000";

    /// <summary>
    ///     Gets or sets an optional client-defined reference identifier submitted to FedEx for the request.
    /// </summary>
    /// <remarks>
    ///     The FedEx resolve endpoint does not echo this value back on the resolved address. You cannot use it to
    ///     correlate a batch response with the request that produced it. Results correlate strictly by position
    ///     (see <see cref="AddressValidation.Services.IBatchAddressValidationService{TRequest}" />). FedEx
    ///     transmits this value only for its own tracking purposes.
    /// </remarks>
    public string? ClientReferenceId { get; set; }

    /// <summary>
    ///     Gets or sets the customer transaction ID used to identify the transaction.
    /// </summary>
    /// <remarks>
    ///     When submitted via <see cref="AddressValidation.Services.IBatchAddressValidationService{TRequest}" />,
    ///     the FedEx API accepts only one transaction identifier for the entire batch call. It transmits only the
    ///     value set on the first request in the batch.
    /// </remarks>
    public string? CustomerTransactionId { get; set; }
}
