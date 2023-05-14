using System.Net;

namespace Crezco.Location.Location;

public interface ILocationService
{
    Task<GetLocationByIpResponse?> GetLocationByIpAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
}