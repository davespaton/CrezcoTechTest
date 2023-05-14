﻿using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Crezco.Location.Location;

public interface ILocationService
{
    Task<GetLocationByIpResponse?> GetLocationByIpAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
}

internal sealed class LocationService : ILocationService
{
    private readonly ILocationClient _locationClient;
    private readonly ILocationRepository _locationRepository;
    private readonly ICache<LocationData> _cache;
    private readonly ILogger<ILocationService> _logger;

    public LocationService(
        ILocationClient locationClient,
        ILocationRepository locationRepository,
        ICache<LocationData> cache,
        ILogger<ILocationService> logger)
    {
        _locationClient = locationClient;
        _locationRepository = locationRepository;
        _cache = cache;
        _logger = logger;
    }


    public async Task<GetLocationByIpResponse?> GetLocationByIpAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
    {
        LocationData? location = await _cache.TryGetOrCreateAsync(ipAddress.ToString(), () => GetLocationFromApi(ipAddress, cancellationToken), cancellationToken);

        return location?.Data.Adapt<GetLocationByIpResponse>();
    }

    private async Task<LocationData?> GetLocationFromApi(IPAddress ipAddress, CancellationToken cancellationToken = default)
    {
        LocationApiData? apiResponse = null;
        try
        {
            apiResponse = await _locationClient.GetLocationAsync(ipAddress, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure to get location for {ip}", ipAddress.ToString());
        }

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

        try
        {
            await _locationRepository.CreateAsync(location, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failure to persist location data for {location}", location);
        }

        return location;
    }
}