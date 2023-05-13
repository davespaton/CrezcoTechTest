using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crezco.Infrastructure.Persistence;

public class MongoContext
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public const string LocationCollectionName = "locations";

    public MongoContext(IOptions<DatabaseSettings> dbOptions)
    {
        DatabaseSettings settings = dbOptions.Value;
        _client = new MongoClient(settings.ConnectionString);
        _database = _client.GetDatabase(settings.DatabaseName);

        EnsureIndexesCreated();
    }

    private void EnsureIndexesCreated()
    {
        IMongoCollection<Location>? collection = _database.GetCollection<Location>(LocationCollectionName);

        IndexKeysDefinition<Location>? ipKey = Builders<Location>.IndexKeys.Ascending(x => x.IpAddress);
        IndexKeysDefinition<Location>? createdAtKey = Builders<Location>.IndexKeys.Descending(x => x.CreatedAt);
        IndexKeysDefinition<Location>? compoundKey = Builders<Location>.IndexKeys.Combine(ipKey, createdAtKey);

        collection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Location>(ipKey),
            new CreateIndexModel<Location>(createdAtKey),
            new CreateIndexModel<Location>(compoundKey)
        });
    }

    public IMongoClient Client => _client;

    public IMongoDatabase Database => _database;
}