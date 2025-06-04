namespace Visus.AddressValidation.SourceGeneration.Tests;

using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

public static class ModuleInitializer
{
    #pragma warning disable CA2255
    [ModuleInitializer]
    #pragma warning restore CA2255
    public static void Initialize()
    {
        DerivePathInfo((file, _, type, method) => new PathInfo(Path.Combine(Path.GetDirectoryName(file)!, "ref"), type.Name, method.Name));

        VerifySourceGenerators.Initialize();
        VerifyDiffPlex.Initialize(OutputType.Compact);
    }
}
