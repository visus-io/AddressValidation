namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Contracts;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSourceGenerationOptions(WriteIndented = false)]
internal sealed partial class ApiResponseJsonSerializerContext : JsonSerializerContext;
