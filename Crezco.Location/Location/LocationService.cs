using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Repositories;
using Mapster;
using System.Net;

namespace Crezco.Location.Location;

internal sealed class LocationService : ILocationService
{
    private readonly ILocationClient _locationClient;
    private readonly ILocationRepository _locationRepository;
    private readonly ICache<LocationData> _cache;

    public LocationService(
        ILocationClient locationClient,
        ILocationRepository locationRepository,
        ICache<LocationData> cache)
    {
        _locationClient = locationClient;
        _locationRepository = locationRepository;
        _cache = cache;
    }

    public async Task<GetLocationByIpResponse?> GetLocationByIpAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
    {
        LocationData? location = await _cache.TryGetOrCreateAsync(ipAddress.ToString(), () => GetLocationFromApi(ipAddress, cancellationToken), cancellationToken);

        return location?.Data.Adapt<GetLocationByIpResponse>();
    }

    private async Task<LocationData?> GetLocationFromApi(IPAddress ipAddress, CancellationToken cancellationToken = default)
    {
        LocationApiData? apiResponse = await _locationClient.GetLocationAsync(ipAddress, cancellationToken);
        
        if (apiResponse is null)
        {
            return null;
        }

        var location = new LocationData()
        {
            CreatedAt = DateTime.UtcNow,
            Data = apiResponse,
            IpAddress = ipAddress.ToString()
        };

        await _locationRepository.CreateAsync(location, cancellationToken);

        return location;
    }
}