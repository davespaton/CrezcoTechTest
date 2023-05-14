using Crezco.Infrastructure.Cache;

namespace Crezco.Location.Tests;

public class MockCache<T> : ICache<T> where T : class
{
    private T? _data;
    private bool _cacheHit = true;

    public void SetCache(T? data, bool cacheHit = true)
    {
        _data = data;
        _cacheHit = cacheHit;
    }

    public Task<T?> TryGetOrCreateAsync(string key, Func<Task<T?>> create, CancellationToken cancellationToken = default)
    {
        return !_cacheHit ? create() : Task.FromResult(_data);
    }
}