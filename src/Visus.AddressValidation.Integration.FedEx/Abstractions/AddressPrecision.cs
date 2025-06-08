namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum AddressPrecision
{
    /// <summary>
    ///     Indicates address has valid secondary information
    /// </summary>
    MULTI_TENANT_UNIT,

    /// <summary>
    ///     Indicates address is a valid multi-tenant location but could not resolve secondary information.
    /// </summary>
    MULTI_TENANT_BASE,

    /// <summary>
    ///     Indicates the local postal authority services the address.
    /// </summary>
    PO_BOX,

    /// <summary>
    ///     Indicates the address contains a unique postal code for USPS
    /// </summary>
    UNIQUE_ZIP,

    /// <summary>
    ///     Indicates the location is not a valid multi-tenant location
    /// </summary>
    STREET_ADDRESS
}
