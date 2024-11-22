namespace Visus.AddressValidation.SourceGeneration.Tests;

public sealed class CustomResponseDataGeneratorFacts
{
	[Fact]
	public Task ShouldEmitForNestedType()
	{
		const string source = """
							  namespace Visus.AddressValidation.SourceGeneration.Tests;

							  partial class Response
							  {
							      public string FirstName { get; set; }
							      
							      public string LastName { get; set; }
							      
							      partial class ApiData
							      {
							          [CustomResponseDataProperty]
							          public string ResponseId { get; set; }
							          
							          [CustomResponseDataProperty("response")]
							          public string ResponseText { get; set; }
							      }
							  }
							  """;

		return Fixture.VerifyGenerateSourcesAsync(source, new CustomResponseDataGenerator());
	}

	[Fact]
	public Task ShouldEmitForRootType()
	{
		const string source = """
							  namespace Visus.AddressValidation.SourceGeneration.Tests;

							  partial class ApiResponse
							  {
							      [CustomResponseDataProperty]
							      public string ResponseId { get; set; }
							  
							      [CustomResponseDataProperty("response")]
							      public string ResponseText { get; set; }
							  }
							  """;

		return Fixture.VerifyGenerateSourcesAsync(source, new CustomResponseDataGenerator());
	}
}
