namespace Visus.AddressValidation.Integration.Ups.Models;

using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Representation of a unified address validation request to UPS.
/// </summary>
[UsedImplicitly]
public sealed class UpsAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets or sets the maximum number of address candidates to return.
    ///     Must be between 0 and 50. When <see langword="null" />, the UPS API
    ///     default of 15 is used.
    /// </summary>
    public int? MaximumCandidateListSize { get; set; }
}
