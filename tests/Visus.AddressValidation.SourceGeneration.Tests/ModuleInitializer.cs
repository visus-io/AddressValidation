namespace Visus.AddressValidation.SourceGeneration.Tests;

using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifySourceGenerators.Initialize();
        VerifyDiffPlex.Initialize(OutputType.Compact);
    }
}
