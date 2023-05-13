using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.Cache.Helpers;
using Crezco.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Crezco.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection BindInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ICacheHelper, CacheHelper>();

        // this could be some sort of reflection for all caches
        collection.AddSingleton<ICache<Location>, GetLocationCache>();

        return collection;
    }
}
