using System.Net;
using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Repositories;
using Crezco.Location.Location;
using Crezco.Location.Tests.Builders;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Crezco.Location.Tests;

public class LocationServiceUnitTests
{
    private readonly ILocationClient _locationClient = A.Fake<ILocationClient>();
    private readonly ILocationRepository _locationRepository = A.Fake<ILocationRepository>();
    private readonly ILogger<ILocationService> _logger = A.Fake<ILogger<ILocationService>>();

    private readonly MockCache<LocationData> _cache = new();

    private LocationService GetLocation() => new(_locationClient, _locationRepository, _cache);

    void SetApiResponse(LocationApiData apiData)
        => A.CallTo(() => _locationClient.GetLocationAsync(A<IPAddress>._, A<CancellationToken>._))
            .Returns(apiData);

    [Fact]
    public async Task GettingLocation_ExistsInCache_DoesNotWriteToDb()
    {
        // Arrange
        LocationService locationService = GetLocation();
        IPAddress ipAddress = IPAddress.Parse("1.1.1.1");

        var apiBuilder = new LocationApiDataBuilder();
        SetApiResponse(apiBuilder.Build());

        LocationData locationData = new LocationDataBuilder(ipAddress.ToString(), apiBuilder)
            .Build();

        _cache.SetCache(locationData);

        // Act
        _ = await locationService.GetLocationByIpAsync(ipAddress);

        // Assert
        A.CallTo(() => _locationRepository.CreateAsync(A<LocationData>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task GettingLocation_DoesNotExistInCache_WritesToDb()
    {
        // Arrange
        LocationService locationService = GetLocation();
        IPAddress ipAddress = IPAddress.Parse("1.1.1.1");

        var apiBuilder = new LocationApiDataBuilder();
        SetApiResponse(apiBuilder.Build());
        
        _cache.SetCache(null, false);

        // Act
        _ = await locationService.GetLocationByIpAsync(ipAddress);

        // Assert
        A.CallTo(() => _locationRepository.CreateAsync(A<LocationData>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GettingLocation_FromCache_MapsLocationToResponse()
    {
        // Arrange
        LocationService locationService = GetLocation();
        IPAddress ipAddress = IPAddress.Parse("1.1.1.1");

        LocationApiData apiResponse = new LocationApiDataBuilder()
            .WithCity("Tokyo")
            .Build();
        SetApiResponse(apiResponse);

        LocationApiDataBuilder cacheResponseBuilder = new LocationApiDataBuilder()
            .WithCity("London");
        LocationData cacheResponse = new LocationDataBuilder(ipAddress.ToString(), cacheResponseBuilder)
            .Build();
        _cache.SetCache(cacheResponse);

        // Act
        GetLocationByIpResponse? response = await locationService.GetLocationByIpAsync(ipAddress);

        // Assert
        AssertResponse(response, cacheResponse.Data);
    }

    [Fact]
    public async Task GettingLocation_FromApi_MapsLocationToResponse()
    {
        // Arrange
        LocationService locationService = GetLocation();
        IPAddress ipAddress = IPAddress.Parse("1.1.1.1");

        LocationApiData apiResponse = new LocationApiDataBuilder()
            .WithCity("Tokyo")
            .Build();
        SetApiResponse(apiResponse);

        LocationApiDataBuilder cacheResponseBuilder = new LocationApiDataBuilder()
            .WithCity("London");
        LocationData cacheResponse = new LocationDataBuilder(ipAddress.ToString(), cacheResponseBuilder)
            .Build();
        _cache.SetCache(cacheResponse, false);

        // Act
        GetLocationByIpResponse? response = await locationService.GetLocationByIpAsync(ipAddress);

        // Assert
        AssertResponse(response, apiResponse);
    }

    private static void AssertResponse(GetLocationByIpResponse? response, LocationApiData data)
    {
        response.Should().NotBeNull();

#pragma warning disable CS8602
        response.CityName.Should().Be(data.CityName);
        response.CountryName.Should().Be(data.CountryName);
        response.Latitude.Should().Be(data.Latitude);
        response.Longitude.Should().Be(data.Longitude);
        response.RegionName.Should().Be(data.RegionName);
#pragma warning restore CS8602
    }

}
