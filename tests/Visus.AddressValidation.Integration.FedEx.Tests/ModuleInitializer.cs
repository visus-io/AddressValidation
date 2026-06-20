namespace Visus.AddressValidation.Integration.FedEx.Tests;

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
