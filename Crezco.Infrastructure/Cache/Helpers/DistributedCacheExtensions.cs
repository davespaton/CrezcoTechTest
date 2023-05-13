using Microsoft.Extensions.Caching.Distributed;

namespace Crezco.Infrastructure.Cache.Helpers;
internal static class DistributedCacheExtensions
{
    public static async Task<CacheWrapper<T>?> GetAsync<T>(
        this IDistributedCache cache,
        string key,
        CancellationToken cancellationToken = default) where T : class
    {
        byte[]? data = await cache.GetAsync(key, cancellationToken);
        if (data is null)
            return null;

        return CacheWrapper<T>.FromByteArray(data);
    }

    public static async Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        CacheWrapper<T> value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        byte[] data = value.ToByteArray();
        await cache.SetAsync(key, data, options, cancellationToken);
    }
}
