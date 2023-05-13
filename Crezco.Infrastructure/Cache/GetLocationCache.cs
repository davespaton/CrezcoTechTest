using Crezco.Infrastructure.Cache.Helpers;
using Crezco.Infrastructure.Persistence;

namespace Crezco.Infrastructure.Cache;

internal sealed class GetLocationCache: ICache<Location>
{
    private readonly ICacheHelper _cacheHelper;

    private string UniqueKey(string key) => $"GetLocationCache_v1_{key}";

    public GetLocationCache(ICacheHelper cacheHelper)
    {
        _cacheHelper = cacheHelper;
    }

    public Task<Location?> TryGetOrCreateAsync(string key, Func<Task<Location?>> create, CancellationToken cancellationToken = default) => 
        _cacheHelper.TryGetOrCreateAsync(UniqueKey(key), create, cancellationToken);
}