namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using System.Runtime.CompilerServices;
using Http;
using PublicApiGenerator;

public class ApiFacts
{
	[Fact]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public async Task AddressValidation_Google_NoBreakingChanges_Async()
	{
		var api = typeof(PitneyBowesAuthenticationClient).Assembly.GeneratePublicApi(new ApiGeneratorOptions
		{
			ExcludeAttributes = ["System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute"]
		});

		await Verify(api);
	}
}
