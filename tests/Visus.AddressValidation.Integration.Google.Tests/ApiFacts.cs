namespace Visus.AddressValidation.Integration.Google.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Http;
using PublicApiGenerator;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public sealed class ApiFacts
{
    [Fact]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public async Task AddressValidation_Google_NoBreakingChanges_Async()
    {
        var api = typeof(GoogleAddressValidationClient).Assembly.GeneratePublicApi(new ApiGeneratorOptions
        {
            ExcludeAttributes = ["System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute"]
        });

        await Verify(api);
    }
}
