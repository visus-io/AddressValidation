---
title: Introduction | Custom Integration
---

## Introduction

Building a custom integration for **AddressValidation** means implementing a set of well-defined, single-responsibility contracts. The framework handles orchestration — the validation pipeline, HTTP resilience, and token caching — so you only need to supply provider-specific logic.

## Steps

Follow these steps in order to build a custom integration:

1. [Request Model](xref:custom-request-model) — define the public-facing request type consumers will use
2. [Provider API Contracts](xref:custom-contracts) — define DTO classes for the provider's JSON payload
3. [Request Mapper](xref:custom-request-mapper) — translate the public request model into the provider DTO
4. [Response Mapper](xref:custom-response-mapper) — translate the provider response into a unified `IAddressValidationResponse`
5. [Authentication Client](xref:custom-authentication-client) — request an access token from the provider *(OAuth 2.0 only)*
6. [Authentication Service](xref:custom-authentication-service) — cache and refresh the access token *(OAuth 2.0 only)*
7. [Validation Client](xref:custom-validation-client) — call the provider's address validation endpoint
8. [Request Adapter](xref:custom-request-adapter) — bridge the public request to the validation client
9. [Validators](xref:custom-validators) — validate the request before sending and the response after receiving
10. [Validation Service](xref:custom-validation-service) — wire the pipeline components together
11. [Registering Services](xref:custom-registering-services) — register all components with the DI container

> [!NOTE]
> Steps 5 and 6 are only required when the provider uses OAuth 2.0 bearer tokens. Providers that authenticate via static API keys, query string parameters, or other schemes can skip those two steps and omit the [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) from the HTTP client pipeline during [service registration](xref:custom-registering-services).
