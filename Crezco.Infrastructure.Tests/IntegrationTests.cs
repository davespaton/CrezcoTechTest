using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Crezco.Infrastructure.Tests;

public abstract class IntegrationTests: IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly ServiceCollection _serviceDescriptors;

    private string _redisInstance = "Test_" + Guid.NewGuid();

    protected IntegrationTests()
    {
        _serviceDescriptors = new ServiceCollection();
        ConfigureServices(_serviceDescriptors);
        _provider = _serviceDescriptors.BuildServiceProvider();
    }


    protected T GetService<T>() where T : notnull => _provider.GetRequiredService<T>();

    private void ConfigureServices(IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.AddLogging();
        serviceDescriptors.BindInfrastructureServices();

        serviceDescriptors.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost";
            options.InstanceName = _redisInstance;
        });
    }


    public void Dispose()
    {
        // todo connect to redis here and delete the _redistInstance


        _provider?.Dispose();
    }
}