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
    ///     Adds a standard resilience handler that respects the <c>Retry-After</c> response header on
    ///     429 (Too Many Requests) responses, falling back to a 10-second delay when no header is present.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder" /> to configure.</param>
    /// <returns>The same builder so that multiple calls can be chained.</returns>
    public static IHttpClientBuilder AddAddressValidationResilienceHandler(this IHttpClientBuilder builder)
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
}
