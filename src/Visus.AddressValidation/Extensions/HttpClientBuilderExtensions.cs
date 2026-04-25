namespace Visus.AddressValidation.Extensions;

using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

/// <summary>
///     Contains extension methods for <see cref="IHttpClientBuilder" />.
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    ///     Adds a standard resilience handler for the address validation HTTP client.
    ///     On 429 (Too Many Requests) responses, the retry delay honors the <c>Retry-After</c>
    ///     header. The <c>delta-seconds</c> form is preferred; the absolute date/time form is used
    ///     as a fallback. When no header is present, a 10-second delay is applied.
    ///     All other status codes use the default retry delay strategy.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder" /> to configure.</param>
    /// <returns>The same builder so that multiple calls can be chained.</returns>
    public static IHttpClientBuilder AddAddressValidationClientResilienceHandler(this IHttpClientBuilder builder)
    {
        builder.AddStandardResilienceHandler(options =>
        {
            options.Retry.DelayGenerator = static args =>
            {
                if ( args.Outcome.Result?.StatusCode != HttpStatusCode.TooManyRequests )
                {
                    return new ValueTask<TimeSpan?>(default(TimeSpan?));
                }

                RetryConditionHeaderValue? retryAfter = args.Outcome.Result.Headers.RetryAfter;

                if ( retryAfter?.Delta is { } delta )
                {
                    return new ValueTask<TimeSpan?>(delta);
                }

                if ( retryAfter?.Date is not { } date )
                {
                    return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(10));
                }

                TimeSpan remaining = date - DateTimeOffset.UtcNow;
                return new ValueTask<TimeSpan?>(remaining > TimeSpan.Zero ? remaining : TimeSpan.FromSeconds(10));
            };
        });

        return builder;
    }

    /// <summary>
    ///     Adds a standard resilience handler configured for authentication clients, enabling retries only for safe
    ///     HTTP methods and lowering the circuit-breaker minimum throughput threshold to 5 requests.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder" /> to configure.</param>
    /// <returns>The same builder so that multiple calls can be chained.</returns>
    public static IHttpClientBuilder AddAuthenticationClientResilienceHandler(this IHttpClientBuilder builder)
    {
        builder.AddStandardResilienceHandler(options =>
        {
            options.Retry.DisableForUnsafeHttpMethods();
            options.CircuitBreaker.MinimumThroughput = 5;
        });

        return builder;
    }
}
