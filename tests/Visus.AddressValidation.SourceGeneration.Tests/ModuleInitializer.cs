namespace Visus.AddressValidation.SourceGeneration.Tests;

using JetBrains.Annotations;
using VerifyTests.DiffPlex;

[UsedImplicitly]
internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifySourceGenerators.Initialize();
        VerifyDiffPlex.Initialize(OutputType.Compact);
    }
}
