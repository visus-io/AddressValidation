namespace Visus.AddressValidation.Abstractions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///     Enumeration of potential client environments
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ClientEnvironment
{
    /// <summary>
    ///     Development
    /// </summary>
    [Description("Development")]
    DEVELOPMENT = 0,

    /// <summary>
    ///     Production
    /// </summary>
    [Description("Production")]
    PRODUCTION = 1,
}
