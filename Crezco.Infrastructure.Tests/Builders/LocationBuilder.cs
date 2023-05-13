﻿using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Persistence;

namespace Crezco.Infrastructure.Tests.Builders;

public class LocationBuilder
{
    private readonly string _ipAddress;
    private string? _cityName;

    public LocationBuilder(string ipAddress)
    {
        _ipAddress = ipAddress;
    }

    public LocationBuilder WithCityName(string? cityName)
    {
        _cityName = cityName; return this;
    }

    public Location Build() =>
        new()
        {
            CreatedAt = DateTime.UtcNow,
            IpAddress = _ipAddress,
            Data = new LocationApiData
            {
                CityName = _cityName ?? "London",
                CountryCode = "GB",
                CountryName = "United Kingdom",
                Latitude = 0.12,
                Longitude = 2.34,
                RegionName = "England",
                TimeZone = "g",
                ZipCode = "ME23"
            }
        };
}