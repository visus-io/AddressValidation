using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability",
                           "CA2007:Do not directly await a Task",
                           Justification = "Discouraged for Blazor UI Components",
                           Scope = "namespaceanddescendants",
                           Target = "~N:AddressValidation.Demo.Common")]

[assembly: SuppressMessage("Reliability",
                           "CA2007:Do not directly await a Task",
                           Justification = "Discouraged for Blazor UI Components",
                           Scope = "namespaceanddescendants",
                           Target = "~N:AddressValidation.Demo.Features")]
