using Crezco.Infrastructure.Cache.Helpers;
using Crezco.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Distributed;

namespace Crezco.Infrastructure.Cache;

internal sealed class GetLocationCache: ICache<LocationData>
{
    private readonly ICacheHelper _cacheHelper;

    private static string UniqueKey(string key) => $"GetLocationCache_v1_{key}";

    public GetLocationCache(ICacheHelper cacheHelper)
    {
        _cacheHelper = cacheHelper;
    }

    public static DistributedCacheEntryOptions DistributedCacheEntryOptions = new()
    {
        // todo this should come from a config file
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
    };

    public Task<LocationData?> TryGetOrCreateAsync(string key, Func<Task<LocationData?>> create, CancellationToken cancellationToken = default) => 
        _cacheHelper.TryGetOrCreateAsync(UniqueKey(key), create, DistributedCacheEntryOptions, cancellationToken);
}