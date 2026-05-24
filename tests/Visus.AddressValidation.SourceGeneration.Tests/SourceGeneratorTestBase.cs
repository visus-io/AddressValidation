namespace Visus.AddressValidation.SourceGeneration.Tests;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal abstract class SourceGeneratorTestBase
{
    protected abstract IEnumerable<Type> RequiredAssemblies { get; }

    private IEnumerable<MetadataReference> AssemblyReferencesForCodeGen =>
        AppDomain.CurrentDomain
                 .GetAssemblies()
                 .Concat(new[]
                  {
                      typeof(Binder),
                  }.Concat(RequiredAssemblies).Select(s => s.Assembly))
                 .Distinct()
                 .Where(w => !w.IsDynamic)
                 .Select(s => MetadataReference.CreateFromFile(s.Location));

    protected Task VerifyGenerateSourcesAsync(string source, params IIncrementalGenerator[] generators)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default);

        CSharpCompilation compilation = CSharpCompilation.Create(
            "compilation",
            [syntaxTree,],
            AssemblyReferencesForCodeGen,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generators);
        GeneratorDriver runner = driver.RunGenerators(compilation);
        SettingsTask verify = Verify(runner);

        return verify;
    }
}
