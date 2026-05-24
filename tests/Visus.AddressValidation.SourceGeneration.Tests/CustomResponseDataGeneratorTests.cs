extern alias sourcegen;

namespace Visus.AddressValidation.SourceGeneration.Tests;

using CustomResponseDataGenerator = sourcegen::Visus.AddressValidation.SourceGeneration.CustomResponseDataGenerator;

internal sealed class CustomResponseDataGeneratorTests : SourceGeneratorTestBase
{
    protected override IEnumerable<Type> RequiredAssemblies =>
    [
        typeof(CustomResponseDataGenerator),
        typeof(CustomResponseDataPropertyAttribute),
    ];

    [Test]
    public Task Should_Generate_GetCustomResponseData_For_Nested_Class()
    {
        const string source = """
                              using Visus.AddressValidation;
                              
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
        
        return VerifyGenerateSourcesAsync(source, new CustomResponseDataGenerator());
    }

    [Test]
    public Task Should_Generate_GetCustomResponseData_For_Root_Class()
    {
        const string source = """
                              using Visus.AddressValidation;
                              
                              namespace Visus.AddressValidation.SourceGeneration.Tests;

                              partial class ApiResponse
                              {
                                  [CustomResponseDataProperty]
                                  public string ResponseId { get; set; }

                                  [CustomResponseDataProperty("response")]
                                  public string ResponseText { get; set; }
                              }
                              """;
        
        return VerifyGenerateSourcesAsync(source, new CustomResponseDataGenerator());
    }
}
