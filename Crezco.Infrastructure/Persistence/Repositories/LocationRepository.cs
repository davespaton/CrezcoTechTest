using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Crezco.Infrastructure.Persistence.Repositories;

internal class LocationRepository : ILocationRepository
{
    private readonly IMongoCollection<LocationData> _collection;

    public LocationRepository(IMongoDatabase mongoDatabase)
    {
        _collection = mongoDatabase.GetCollection<LocationData>(MongoContext.LocationCollectionName);
    }

    public async Task<LocationData?> GetLatestAsync(string ipAddress, CancellationToken cancellationToken = default) =>
        await _collection.AsQueryable()
            .Where(r => r.IpAddress == ipAddress)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task CreateAsync(LocationData data, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(data, cancellationToken: cancellationToken);
    }
}
