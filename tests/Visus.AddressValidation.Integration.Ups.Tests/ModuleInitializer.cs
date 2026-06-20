namespace Visus.AddressValidation.Integration.Ups.Tests;

using JetBrains.Annotations;
using VerifyTests.DiffPlex;

[UsedImplicitly]
internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
    }
}
