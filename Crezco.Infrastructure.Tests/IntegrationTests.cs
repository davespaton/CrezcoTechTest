using Crezco.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Crezco.Infrastructure.Tests;

public abstract class IntegrationTests: IDisposable
{
    protected readonly ServiceProvider Provider;

    private const string RedisConfiguration = "localhost:6379";
    private readonly string _redisInstance = "test_" + Guid.NewGuid();

    private const string MongoConnection = "mongodb://localhost/27017";
    private readonly string _mongoDatabaseName = "test_db_" + Guid.NewGuid();

    private MongoContext? _mongoContext;

    protected IntegrationTests()
    {
        var serviceDescriptors = new ServiceCollection();
        ConfigureServices(serviceDescriptors);
        Provider = serviceDescriptors.BuildServiceProvider();
    }
    
    protected T GetService<T>() where T : notnull => Provider.GetRequiredService<T>();

    private void ConfigureServices(IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.AddLogging();
        serviceDescriptors.AddInfrastructureServices();

        serviceDescriptors.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = RedisConfiguration;
            options.InstanceName = _redisInstance;
        });

        IOptions<DatabaseOptions> options = Options.Create(new DatabaseOptions()
        {
            ConnectionString = MongoConnection,
            DatabaseName = _mongoDatabaseName
        });

        _mongoContext = new MongoContext(options);

        serviceDescriptors.AddScoped<IMongoDatabase>((_) => _mongoContext.Database);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) 
            return;

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisConfiguration);
        foreach (RedisKey key in redis.GetServer(RedisConfiguration).Keys(pattern: _redisInstance + "*"))
        {
            redis.GetDatabase().KeyDelete(key);
        }

        _mongoContext?.Client.DropDatabase(_mongoDatabaseName);

        Provider?.Dispose();
    }
}