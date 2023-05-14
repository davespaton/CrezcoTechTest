using Crezco.Infrastructure;
using Crezco.Location.Location;
using Microsoft.Extensions.DependencyInjection;

namespace Crezco.Location;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocationServices(this IServiceCollection collection)
    {
        collection.AddScoped<ILocationService, LocationService>();
        
        collection.AddInfrastructureServices();

        return collection;
    }
}
