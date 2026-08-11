namespace Visus.AddressValidation.Integration.Ups.Models;

using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Represents a unified address validation request sent to UPS.
/// </summary>
[UsedImplicitly]
public sealed class UpsAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets or sets the maximum number of address candidates to return. It
    ///     must be between 0 and 50. When <see langword="null" />, UPS applies
    ///     its default of 15.
    /// </summary>
    public int? MaximumCandidateListSize { get; set; }
}
