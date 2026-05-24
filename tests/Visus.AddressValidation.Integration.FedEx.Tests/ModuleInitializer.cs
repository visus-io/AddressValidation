namespace Visus.AddressValidation.Integration.FedEx.Tests;

using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
    }
}
