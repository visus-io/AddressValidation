---
title: Registering Services
uid: custom-registering-services
---

## Registering Services

After implementing all components, wire them together in a static extension method on [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection). This is the pattern used by all built-in integrations and is the single call a consumer makes at application startup.

## Validation Service

The validation service wires all pipeline components together. Extend [`AbstractAddressValidationService<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Services.AbstractAddressValidationService`2) and forward the four constructor dependencies to the base class.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationService : AbstractAddressValidationService<MyAddressValidationRequest, ApiResponse>
{
    public AddressValidationService(IApiRequestAdapter<MyAddressValidationRequest, ApiResponse> requestAdapter,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IValidator<MyAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
    }
}
```

> [!NOTE]
> No additional logic belongs here. The base class manages the full validation pipeline: pre-validating the request, calling the API via the [request adapter](xref:custom-validation-client), validating the response, and mapping it to an [`IAddressValidationResponse`](xref:Visus.AddressValidation.Models.IAddressValidationResponse).

> [!NOTE]
> It is not necessary for the validation service to be `internal`, but it is **strongly** recommended if redistributing as a library.

## Options

Define a configuration options class by extending [`AbstractServiceOptions`](xref:Visus.AddressValidation.Configuration.AbstractServiceOptions). The base class provides `ClientEnvironment` and the optional `EndpointUriOverride`; you only need to implement the abstract `EndpointUri` property and declare any provider-specific required fields.

```csharp
public sealed class MyServiceOptions : AbstractServiceOptions
{
    public const string SectionName = "AddressValidationSettings:MyProvider";

    public override Uri EndpointUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.PRODUCTION => new Uri("https://api.myprovider.example.com"),
            ClientEnvironment.SANDBOX    => EndpointUriOverride!,
            _                            => new Uri("https://api-dev.myprovider.example.com"),
        };

    [Required(AllowEmptyStrings = false)]
    public required string ClientId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public required string ClientSecret { get; set; }
}
```

Alongside the options class, add a source-generated validator:

```csharp
[OptionsValidator]
public sealed partial class MyServiceOptionsValidator : IValidateOptions<MyServiceOptions> { }
```

> [!NOTE]
> The `[OptionsValidator]` attribute generates validation of all `[Required]` and other data-annotation attributes declared on the options class at compile time. No manual validation code is needed unless cross-property rules are required; the cross-property `SANDBOX`/`EndpointUriOverride` rule is already handled by [`AbstractServiceOptions`](xref:Visus.AddressValidation.Configuration.AbstractServiceOptions).

## Batch Validation Service (Optional)

> [!NOTE]
> This section only applies when the provider's API natively supports validating multiple addresses in a single call. Skip it if the provider only offers a single-address endpoint.

Extend [`AbstractBatchAddressValidationService<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Services.AbstractBatchAddressValidationService`2) and forward its four constructor dependencies to the base class, alongside the provider's maximum batch size.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchAddressValidationService : AbstractBatchAddressValidationService<MyAddressValidationRequest, ApiResponse>
{
    public BatchAddressValidationService(IBatchApiRequestAdapter<MyAddressValidationRequest, ApiResponse> batchRequestAdapter,
                                         IBatchApiResponseMapper<ApiResponse> batchResponseMapper,
                                         IValidator<MyAddressValidationRequest> requestValidator,
                                         IBatchValidator<ApiResponse> batchResponseValidator)
        : base(batchRequestAdapter, batchResponseMapper, requestValidator, batchResponseValidator)
    {
    }

    protected override int MaxBatchSize => Constants.MaxBatchSize;
}
```

> [!NOTE]
> `requestValidator` is the same [`IValidator<TRequest>`](xref:custom-validators) used by the singular pipeline; there is no separate batch request validator. Each request is still validated individually before the batch call is made. See [Batch Validators](xref:custom-batch-validators) for the response side, and [Batch Mappers](xref:custom-batch-mappers) and [Batch Validation Client](xref:custom-batch-validation-client) for the remaining components.

Implement the required components (batch request adapter, batch request mapper, batch response mapper, batch response validator) as described in [Batch Mappers](xref:custom-batch-mappers), [Batch Validation Client](xref:custom-batch-validation-client), and [Batch Validators](xref:custom-batch-validators), then register them alongside the singular pipeline's registrations inside the same extension method:

```csharp
services.TryAddScoped<IBatchApiResponseMapper<ApiResponse>, BatchAddressValidationResponseMapper>();
services.TryAddScoped<IBatchApiRequestMapper<MyAddressValidationRequest, ApiRequest>, BatchAddressValidationRequestMapper>();
services.TryAddScoped<IBatchApiRequestAdapter<MyAddressValidationRequest, ApiResponse>, BatchApiRequestAdapter>();

services.TryAddScoped<IBatchValidator<ApiResponse>, BatchApiResponseValidator>();

services.TryAddScoped<IBatchAddressValidationService<MyAddressValidationRequest>, BatchAddressValidationService>();
```

> [!NOTE]
> There is no separate `Add*BatchAddressValidation()` extension method. A single call to `AddMyProviderAddressValidation()` registers both `IAddressValidationService<MyAddressValidationRequest>` and `IBatchAddressValidationService<MyAddressValidationRequest>` in the same container, since both share the same options, HTTP client, and authentication service.

## Extension Method

Register all components using a single `IServiceCollection` extension method:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyProviderAddressValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<MyServiceOptions>()
                .BindConfiguration(MyServiceOptions.SectionName)
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<MyServiceOptions>, MyServiceOptionsValidator>();

        services.TryAddSingleton<MyAuthenticationService>();

        services.TryAddScoped<IApiResponseMapper<ApiResponse>, AddressValidationResponseMapper>();
        services.TryAddScoped<IApiRequestMapper<MyAddressValidationRequest, ApiRequest>, AddressValidationRequestMapper>();

        services.TryAddScoped<IValidator<MyAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();
        services.TryAddScoped<IApiRequestAdapter<MyAddressValidationRequest, ApiResponse>, ApiRequestAdapter>();

        services.TryAddScoped<IAddressValidationService<MyAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<MyAuthenticationClient>()
                .AddAuthenticationClientResilienceHandler();

        services.AddHttpClient<MyAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     MyAuthenticationService authenticationService = provider.GetRequiredService<MyAuthenticationService>();
                     return new BearerTokenDelegatingHandler<MyAuthenticationClient>(authenticationService);
                 })
                .AddAddressValidationClientResilienceHandler();

        return services;
    }
}
```

> [!IMPORTANT]
> Use `TryAddSingleton` and `TryAddScoped` rather than `AddSingleton` and `AddScoped`. This prevents double-registration if the extension method is called more than once and allows consumers to substitute their own implementations before calling it.

> [!NOTE]
> The authentication service is registered as `Singleton` because it holds the [`HybridCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.hybrid.hybridcache)-backed access token and must share that state across the application lifetime. All other integration services are `Scoped`.

[!INCLUDE [hybrid-cache-required](../../includes/hybrid-cache-required.md)]

> [!NOTE]
> `ValidateOnStart()` surfaces configuration errors (such as missing required properties or an invalid `SANDBOX`/`EndpointUriOverride` combination) immediately at application startup rather than on the first validation request.

> [!IMPORTANT]
> The type argument to [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) must match the exact authentication client type registered via `AddHttpClient<TClient>()`. A mismatch results in a runtime failure when the DI container tries to resolve the handler.

## Usage

Once registered, inject [`IAddressValidationService<TRequest>`](xref:Visus.AddressValidation.Services.IAddressValidationService`1) wherever address validation is needed:

```csharp
public class ValidateController
{
    private readonly IAddressValidationService<MyAddressValidationRequest> _validationService;

    public ValidateController(IAddressValidationService<MyAddressValidationRequest> validationService)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MyAddressValidationRequest request, CancellationToken cancellationToken = default)
    {
        IAddressValidationResponse? response = await _validationService.ValidateAsync(request, cancellationToken);

        return response is null
            ? new NotFoundResult()
            : response.Errors.Count > 0
                ? new UnprocessableEntityObjectResult(response)
                : new OkObjectResult(response);
    }
}
```

## Batch Usage

If the [batch validation service](#batch-validation-service-optional) was registered, inject [`IBatchAddressValidationService<TRequest>`](xref:Visus.AddressValidation.Services.IBatchAddressValidationService`1) wherever multiple addresses need to be validated in one call:

```csharp
public class BatchValidateController
{
    private readonly IBatchAddressValidationService<MyAddressValidationRequest> _batchValidationService;

    public BatchValidateController(IBatchAddressValidationService<MyAddressValidationRequest> batchValidationService)
    {
        _batchValidationService = batchValidationService ?? throw new ArgumentNullException(nameof(batchValidationService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] IReadOnlyList<MyAddressValidationRequest> requests, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IAddressValidationResponse?> responses = await _batchValidationService.ValidateManyAsync(requests, cancellationToken);

        return new OkObjectResult(responses);
    }
}
```

> [!NOTE]
> The returned list is positionally aligned with `requests`, not a single response, so there is no single status code to branch on. See [`IBatchAddressValidationService<TRequest>`](xref:Visus.AddressValidation.Services.IBatchAddressValidationService`1) for the full null/`EmptyAddressValidationResponse` semantics per entry.
