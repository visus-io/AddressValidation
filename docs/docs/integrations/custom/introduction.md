---
title: Introduction | Custom Integration
---

## Introduction

Building a custom integration for **AddressValidation** means implementing a set of well-defined, single-responsibility contracts. The framework handles orchestration — the validation pipeline, HTTP resilience, and token caching — so you only need to supply provider-specific logic.

## Steps

Follow these steps in order to build a custom integration:

1. [Data Models](xref:custom-models) — define the public-facing request type and the provider's JSON DTO classes
2. [Mappers](xref:custom-mappers) — translate between the request model and the provider DTO, and map the provider response to `IAddressValidationResponse`
3. [Authentication](xref:custom-authentication) — request, cache, and refresh an access token from the provider *(OAuth 2.0 only)*
4. [Validation Client](xref:custom-validation-client) — call the provider's address validation endpoint and bridge the public request to the client
5. [Validators](xref:custom-validators) — validate the request before sending and the response after receiving
6. [Registering Services](xref:custom-registering-services) — wire the pipeline components together and register all components with the DI container

> [!NOTE]
> Step 3 is only required when the provider uses OAuth 2.0 bearer tokens. Providers that authenticate via static API keys, query string parameters, or other schemes can skip that step and omit the [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) from the HTTP client pipeline during [service registration](xref:custom-registering-services).
