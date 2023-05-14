using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crezco.Infrastructure.Persistence;

public sealed class MongoContext
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public const string LocationCollectionName = "locations";

    public MongoContext(IOptions<DatabaseOptions> dbOptions)
    {
        DatabaseOptions options = dbOptions.Value;
        _client = new MongoClient(options.ConnectionString);
        _database = _client.GetDatabase(options.DatabaseName);

        EnsureIndexesCreated();
    }

    private void EnsureIndexesCreated()
    {
        IMongoCollection<LocationData>? collection = _database.GetCollection<LocationData>(LocationCollectionName);

        IndexKeysDefinition<LocationData>? ipKey = Builders<LocationData>.IndexKeys.Ascending(x => x.IpAddress);
        IndexKeysDefinition<LocationData>? createdAtKey = Builders<LocationData>.IndexKeys.Descending(x => x.CreatedAt);
        IndexKeysDefinition<LocationData>? compoundKey = Builders<LocationData>.IndexKeys.Combine(ipKey, createdAtKey);

        collection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<LocationData>(ipKey),
            new CreateIndexModel<LocationData>(createdAtKey),
            new CreateIndexModel<LocationData>(compoundKey)
        });
    }

    public IMongoClient Client => _client;

    public IMongoDatabase Database => _database;
}