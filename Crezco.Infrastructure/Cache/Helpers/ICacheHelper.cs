namespace Crezco.Infrastructure.Cache.Helpers;

public interface ICacheHelper
{
    /// <summary>
    /// Try and get the value from the cache, if it doesn't exist, set it.
    /// </summary>
    Task<T?> TryGetOrCreateAsync<T>(string key, Func<Task<T?>> create, CancellationToken cancellationToken = default) where T : class;
}