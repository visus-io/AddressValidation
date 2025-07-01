namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum AddressType
{
    /// <summary>
    /// Address country not supported
    /// </summary>
    RAW,
    
    /// <summary>
    /// Address country supported, but unable to match the address against reference data.
    /// </summary>
    NORMAL,
    
    /// <summary>
    /// Address service was able to successfully match the address against reference data.
    /// </summary>
    STANDARDIZED
}
