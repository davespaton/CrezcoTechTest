using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.Cache.Helpers;
using Crezco.Infrastructure.Tests.Extensions;
using Crezco.Infrastructure.Tests.Mocks;
using FakeItEasy;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Crezco.Infrastructure.Tests;

public class CacheHelperUnitTests
{
    private readonly ILogger<ICacheHelper> _logger = A.Fake<ILogger<ICacheHelper>>();
    private readonly IDistributedCache _distributedCache = A.Fake<IDistributedCache>();

    private IOptions<CacheOptions> GetOptions(bool disabled = false) =>
        Options.Create(new CacheOptions { Disabled = disabled, Configuration = "", InstanceName = "" });

    private CacheHelper GetCacheHelper(IOptions<CacheOptions> options) =>
        new(_logger, _distributedCache, options);

    private CacheHelper GetDisabledCache() =>
        GetCacheHelper(GetOptions(true));
    private CacheHelper GetEnabledCache() =>
        GetCacheHelper(GetOptions(false));

    private void SetCacheCacheWrapperResult(string key, CacheWrapper<MockCacheItem>? value) =>
        A.CallTo(() => _distributedCache.GetAsync(key, A<CancellationToken>._))!
            .Returns(Task.FromResult(value?.ToByteArray()));

    private void SetCacheResult(string key, MockCacheItem? value) =>
        A.CallTo(() => _distributedCache.GetAsync(key, A<CancellationToken>._))!
            .Returns(Task.FromResult(new CacheWrapper<MockCacheItem>(value).ToByteArray()));

    [Fact]
    public async Task Cache_Disabled_DoesNotCallGetOrSetCache()
    {
        // Arrange
        CacheHelper cache = GetDisabledCache();
        var key = "key";
        var createItem = new MockCacheItem("create");

        // Act
        MockCacheItem? result = await cache.TryGetOrCreateAsync(key, () => Task.FromResult(createItem)!);

        // Assert
        A.CallTo(() => _distributedCache.GetAsync(key, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _distributedCache.SetAsync(key, A<byte[]>._, A<DistributedCacheEntryOptions>._, A<CancellationToken>._)).MustNotHaveHappened();

        Assert.Equal(createItem, result);
    }

    [Fact]
    public async Task Cache_TryGetOrSet_SetsValueIfMissing()
    {
        // Arrange
        CacheHelper cache = GetEnabledCache();
        var key = "key";
        var createItem = new MockCacheItem("create");
        SetCacheCacheWrapperResult(key, null);

        // Act
        MockCacheItem? result = await cache.TryGetOrCreateAsync(key, () => Task.FromResult(createItem)!);

        // Assert
        A.CallTo(() => _distributedCache.GetAsync(key, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _distributedCache.SetAsync(key, A < byte[]>._, A<DistributedCacheEntryOptions>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        Assert.Equal(createItem, result);
    }

    [Fact]
    public async Task Cache_TryGetOrSet_GetsValueFromCacheIfNotMissing()
    {
        // Arrange
        CacheHelper cache = GetEnabledCache();
        var key = "key";
        var createItem = new MockCacheItem("create");
        var cacheItem = new MockCacheItem("cache");
        SetCacheResult(key, cacheItem);

        // Act
        MockCacheItem? result = await cache.TryGetOrCreateAsync(key, () => Task.FromResult(createItem)!);

        // Assert
        A.CallTo(() => _distributedCache.GetAsync(key, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        Assert.Equal(cacheItem, result);
    }
    
    [Fact]
    public async Task Cache_TryGetOrSet_DoesNotSetValueIfNotMissing()
    {
        // Arrange
        CacheHelper cache = GetEnabledCache();
        var key = "key";
        var createItem = new MockCacheItem("create");
        var cacheItem = new MockCacheItem("cache");
        SetCacheResult(key, cacheItem);

        // Act
        MockCacheItem? result = await cache.TryGetOrCreateAsync(key, () => Task.FromResult(createItem)!);

        // Assert
        A.CallTo(() => _distributedCache.SetAsync(key, A < byte[]>._, A<DistributedCacheEntryOptions>._, A<CancellationToken>._))
            .MustNotHaveHappened();

        Assert.Equal(cacheItem, result);
    }

    [Fact]
    public async Task Cache_TryGetOrSet_LogsErrorIfExceptionSettingValue()
    {
        // Arrange
        CacheHelper cache = GetEnabledCache();
        var key = "key";
        var createItem = new MockCacheItem("create");
        A.CallTo(() => _distributedCache.SetAsync(key, A < byte[]>._, A<DistributedCacheEntryOptions>._, A<CancellationToken>._))
            .Throws(new RedisConnectionException(ConnectionFailureType.SocketClosed, "Failed to connect"));

        // Act
        MockCacheItem? result = await cache.TryGetOrCreateAsync(key, () => Task.FromResult(createItem)!);

        // Assert
        _logger.AssertLog(LogLevel.Error)
            .MustHaveHappenedOnceExactly();

        Assert.Equal(createItem, result);
    }

    [Fact]
    public async Task Cache_TryGetOrSet_LogsErrorIfExceptionGettingValue()
    {
        // Arrange
        CacheHelper cache = GetEnabledCache();
        var key = "key";
        var createItem = new MockCacheItem("create");
        A.CallTo(() => _distributedCache.GetAsync(key, A<CancellationToken>._))
            .Throws(new RedisConnectionException(ConnectionFailureType.SocketClosed, "Failed to connect"));

        // Act
        MockCacheItem? result = await cache.TryGetOrCreateAsync(key, () => Task.FromResult(createItem)!);

        // Assert
        _logger.AssertLog(LogLevel.Error)
            .MustHaveHappenedOnceExactly();

        Assert.Equal(createItem, result);
    }
}
