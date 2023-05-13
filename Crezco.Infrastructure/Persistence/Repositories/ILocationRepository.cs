namespace Crezco.Infrastructure.Persistence.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetLatestAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task CreateAsync(Location data, CancellationToken cancellationToken = default);
}