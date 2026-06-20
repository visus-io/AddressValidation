namespace Visus.AddressValidation.Abstractions;

using System.ComponentModel;

/// <summary>
///     Enumeration of potential client environments
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ClientEnvironment>))]
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

    /// <summary>
    ///     Sandbox (mock/integration test)
    /// </summary>
    [Description("Sandbox")]
    SANDBOX = 2,
}
