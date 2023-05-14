using Crezco.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crezco.Infrastructure.Tests;

public abstract class IntegrationTests: IDisposable
{
    protected readonly ServiceProvider Provider;

    private readonly string _redisInstance = "test_" + Guid.NewGuid();
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
            options.Configuration = "localhost";
            options.InstanceName = _redisInstance;
        });

        IOptions<DatabaseOptions> options = Options.Create(new DatabaseOptions()
        {
            ConnectionString = "mongodb://localhost/27017",
            DatabaseName = _mongoDatabaseName
        });

        _mongoContext = new MongoContext(options);

        serviceDescriptors.AddScoped<IMongoDatabase>((_) => _mongoContext.Database);
    }

    public void Dispose()
    {
        // todo connect to redis here and delete the _redistInstance

        _mongoContext?.Client.DropDatabase(_mongoDatabaseName);

        Provider?.Dispose();
    }
}