namespace Visus.AddressValidation.SourceGeneration.Tests;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal static class Fixture
{
	private static readonly Type[] RequiredAssemblies =
	[
		typeof(Binder),
		typeof(CustomResponseDataPropertyAttribute)
	];

	private static IEnumerable<MetadataReference> AssemblyReferencesForCodeGen =>
		AppDomain.CurrentDomain
				 .GetAssemblies()
				 .Concat(RequiredAssemblies.Select(s => s.Assembly))
				 .Distinct()
				 .Where(w => !w.IsDynamic)
				 .Select(s => MetadataReference.CreateFromFile(s.Location));

	public static Task VerifyGenerateSourcesAsync(string source, params IIncrementalGenerator[] generators)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default);

		var compilation = CSharpCompilation.Create(
												   "compilation",
												   [syntaxTree],
												   AssemblyReferencesForCodeGen,
												   new CSharpCompilationOptions(OutputKind.ConsoleApplication));

		var driver = CSharpGeneratorDriver.Create(generators);
		var runner = driver.RunGenerators(compilation);
		var verify = Verify(runner);

		return verify;
	}
}
