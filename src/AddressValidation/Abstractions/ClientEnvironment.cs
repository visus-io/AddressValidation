namespace Visus.AddressValidation.Abstractions;

using System.ComponentModel;

/// <summary>
///     Enumeration of potential client environments
/// </summary>
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
	PRODUCTION = 1
}
