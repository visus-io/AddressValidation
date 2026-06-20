namespace Visus.AddressValidation.Tests;

using PublicApiGenerator;

internal sealed class ApiTests
{
    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public async Task PublicApi_HasNoBreakingChanges_Async()
    {
        string api = typeof(InvalidImplementationException).Assembly.GeneratePublicApi(new ApiGeneratorOptions
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
