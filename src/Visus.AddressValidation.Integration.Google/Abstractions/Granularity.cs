namespace Visus.AddressValidation.Integration.Google.Abstractions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum Granularity
{
	[Description("Default value. This value is unused.")]
	GRANULARITY_UNSPECIFIED = 0,

	[Description("Below-building level result, such as an apartment.")]
	SUB_PREMISE = 1,

	[Description("Building-level result.")]
	PREMISE = 2,

	[Description("A geocode that approximates the building-level location of the address.")]
	PREMISE_PROXIMITY = 3,

	[Description("The address or geocode indicates a block. Only used in regions which have block-level addressing, such as Japan.")]
	BLOCK = 4,

	[Description("The geocode or address is granular to route, such as a street, road, or highway.")]
	ROUTE = 5,

	[Description("All other granularities, which are bucketed together since they are not deliverable.")]
	OTHER = 6
}
