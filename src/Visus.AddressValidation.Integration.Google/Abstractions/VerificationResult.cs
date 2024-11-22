namespace Visus.AddressValidation.Integration.Google.Abstractions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum VerificationResult
{
	UNKNOWN = 0,
	UNVERIFIED = 1,
	#pragma warning disable CA1707
	PARTIALLY_VERIFIED = 2
	#pragma warning restore CA1707
}
