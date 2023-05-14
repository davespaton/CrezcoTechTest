using Microsoft.Extensions.Caching.Distributed;

namespace Crezco.Infrastructure.Cache.Helpers;

public interface ICacheHelper
{
    /// <summary>
    /// Try and get the value from the cache, if it doesn't exist, set it.
    /// </summary>
    Task<T?> TryGetOrCreateAsync<T>(string key, Func<Task<T?>> create, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default) where T : class;
}