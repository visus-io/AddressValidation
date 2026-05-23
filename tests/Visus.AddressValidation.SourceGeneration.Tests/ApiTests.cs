extern alias sourcegen;

namespace Visus.AddressValidation.SourceGeneration.Tests;

using System.Runtime.CompilerServices;
using PublicApiGenerator;

internal sealed class ApiTests
{
    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public async Task PublicApi_HasNoBreakingChanges_Async()
    {
        string api = typeof(sourcegen::Visus.AddressValidation.SourceGeneration.CustomResponseDataGenerator).Assembly.GeneratePublicApi(new ApiGeneratorOptions
        {
            ExcludeAttributes =
            [
                "System.Reflection.AssemblyMetadataAttribute",
                "System.Runtime.Versioning.TargetFrameworkAttribute",
            ],
        });

        await Verify(api).ConfigureAwait(false);
    }
}
