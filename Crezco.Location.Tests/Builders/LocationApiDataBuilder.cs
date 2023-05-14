using Crezco.Infrastructure.External.LocationClient;

namespace Crezco.Location.Tests.Builders;

public class LocationApiDataBuilder
{
    private string? _city;

    public LocationApiDataBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public LocationApiData Build()
    {
        return new LocationApiData()
        {
            CityName = _city ?? "Tokyo",
            CountryCode = "JP",
            CountryName = "Japan",
            Latitude = 35.6897,
            Longitude = 139.6922,
            RegionName = "Tokyo",
            TimeZone = "Asia/Tokyo",
            ZipCode = "100-0001"
        };
    }
}