using System.Net;

namespace Crezco.Infrastructure.External.LocationClient;

public interface ILocationClient
{
    Task<LocationApiData?> GetLocationAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
}