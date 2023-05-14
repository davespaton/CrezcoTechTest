namespace Crezco.Infrastructure.Persistence.Repositories;

public interface ILocationRepository
{
    Task<LocationData?> GetLatestAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task CreateAsync(LocationData data, CancellationToken cancellationToken = default);
}