namespace Crezco.Infrastructure.Cache;

internal interface ICacheHelper<T>
{
    /// <summary>
    /// Try and get the value from the cache, if it doesn't exist, set it.
    /// </summary>
    Task<T?> TryGetOrCreateAsync(string key, Func<Task<T?>> create, CancellationToken cancellationToken = default);
}