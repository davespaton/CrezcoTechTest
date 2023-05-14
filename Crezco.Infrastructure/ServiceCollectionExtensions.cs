using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.Cache.Helpers;
using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Crezco.Infrastructure;
public static class ServiceCollectionExtensions
{
    // Create the retry policy we want
    private static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy(IServiceProvider serviceProvider) => HttpPolicyExtensions
        .HandleTransientHttpError() // HttpRequestException, 5XX and 408
        .OrResult(x => !x.IsSuccessStatusCode)
        .WaitAndRetryAsync(
            new[]
            {
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)
            },
            (result, _, retryCount, context) =>
            {
                var message = $"Retry {retryCount} implemented with RetryPolicy of {context.PolicyKey} at {context.OperationKey}, due to: {result.Exception}.";

                var logger = serviceProvider.GetService<ILogger<HttpClient>>();
                logger?.LogTrace(message);
            });

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ICacheHelper, CacheHelper>();
        collection.AddHttpClient(FreeApiClient.HttpClientName)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://freeipapi.com/api/");
            })
            .AddPolicyHandler((serviceProvider, _) => RetryPolicy(serviceProvider));

        // this could be some sort of reflection for all caches
        collection.AddSingleton<ICache<LocationData>, GetLocationCache>();
        collection.AddSingleton<ILocationClient, FreeApiClient>();

        collection.AddScoped<ILocationRepository, LocationRepository>();
        
        return collection;
    }
}
