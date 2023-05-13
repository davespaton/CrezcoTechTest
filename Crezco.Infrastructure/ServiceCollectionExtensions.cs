using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.Cache.Helpers;
using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Crezco.Infrastructure;
public static class ServiceCollectionExtensions
{
    // Create the retry policy we want
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError() // HttpRequestException, 5XX and 408
        .WaitAndRetryAsync(
            new[]
            {
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)
            },
            (result, timeSpan, retryCount, context) =>
            {
                var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                          $"of {context.PolicyKey} " +
                          $"at {context.OperationKey}, " +
                          $"due to: {result.Exception}.";

                // todo Log details about the retry to trace
            });

    public static IServiceCollection BindInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ICacheHelper, CacheHelper>();
        collection.AddHttpClient(FreeApiClient.HttpClientName)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://freeipapi.com/api/");
            })
            .AddPolicyHandler(RetryPolicy);

        // this could be some sort of reflection for all caches
        collection.AddSingleton<ICache<Location>, GetLocationCache>();
        collection.AddSingleton<ILocationClient, FreeApiClient>();
        

        return collection;
    }
}
