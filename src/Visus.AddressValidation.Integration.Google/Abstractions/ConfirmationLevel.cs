namespace Visus.AddressValidation.Integration.Google.Abstractions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum ConfirmationLevel
{
	[Description("Default value. This value is unused.")]
	CONFIRMATION_LEVEL_UNSPECIFIED = 0,

	[Description("We were able to verify that this component exists and makes sense in the context of the rest of the address.")]
	CONFIRMED = 1,

	[Description("This component could not be confirmed, but it is plausible that it exists. For example, a street number within a known valid range of numbers on a street where specific house numbers are not known.")]
	UNCONFIRMED_BUT_PLAUSIBLE = 2,

	[Description("This component was not confirmed and is likely to be wrong. For example, a neighborhood that does not fit the rest of the address.")]
	UNCONFIRMED_AND_SUSPICIOUS = 3
}
