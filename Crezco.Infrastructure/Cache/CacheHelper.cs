﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Crezco.Infrastructure.Cache;

internal class CacheHelper<T> : ICacheHelper<T> where T : class
{
    private readonly ILogger _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly IOptions<CacheOptions> _options;

    public CacheHelper(
        ILogger logger,
        IDistributedCache distributedCache,
        IOptions<CacheOptions> options)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _options = options;
    }

    public async Task<T?> TryGetOrCreateAsync(
        string key,
        Func<Task<T?>> create,
        CancellationToken cancellationToken = default)
    {
        if (_options.Value.Disabled)
            return await create();

        T? cachedValue = await TryGetCachedValue(key, cancellationToken);
        if (cachedValue is not null)
            return cachedValue;

        T? value = await create();
        if (value is null)
            return null;

        await TrySetCachedValue(key, cancellationToken, value);

        return value;
    }

    private async Task TrySetCachedValue(string key, CancellationToken cancellationToken, T? value)
    {
        try
        {
            var options = new DistributedCacheEntryOptions();
            await _distributedCache.SetAsync(key, new CacheWrapper<T>(value), options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key {key}", key);
        }
    }

    private async Task<T?> TryGetCachedValue(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            CacheWrapper<T>? cacheWrapper = await _distributedCache.GetAsync<T>(key, cancellationToken);
            if (cacheWrapper is not null)
                return cacheWrapper.Value;

            _logger.LogTrace("Cache miss for key: {key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache for key {key}", key);
        }
        return null;
    }
}