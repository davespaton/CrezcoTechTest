﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Crezco.Infrastructure.Cache.Helpers;

internal sealed class CacheHelper: ICacheHelper
{
    private readonly ILogger<CacheHelper> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly IOptions<CacheOptions> _options;

    public CacheHelper(
        ILogger<CacheHelper> logger,
        IDistributedCache distributedCache,
        IOptions<CacheOptions> options)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _options = options;
    }

    public async Task<T?> TryGetOrCreateAsync<T>(
        string key,
        Func<Task<T?>> create,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default) where T: class
    {
        if (_options.Value.Disabled)
            return await create();

        T? cachedValue = await TryGetCachedValue<T>(key, cancellationToken);
        if (cachedValue is not null)
            return cachedValue;

        T? value = await create();
        if (value is null)
            return null;

        await TrySetCachedValue(key, value, options, cancellationToken);

        return value;
    }

    private async Task TrySetCachedValue<T>(string key, T? value, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
        where T: class
    {
        try
        {
            options ??= new DistributedCacheEntryOptions();
            await _distributedCache.SetAsync(key, new CacheWrapper<T>(value), options, cancellationToken);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error setting cache for key {key}", key);
        }
    }

    private async Task<T?> TryGetCachedValue<T>(string key, CancellationToken cancellationToken = default) 
        where T: class
    {
        try
        {
            CacheWrapper<T>? cacheWrapper = await _distributedCache.GetAsync<T>(key, cancellationToken);
            if (cacheWrapper is not null)
                return cacheWrapper.Value;

            _logger.LogTrace("Cache miss for key: {key}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error getting cache for key {key}", key);
        }
        return null;
    }
}