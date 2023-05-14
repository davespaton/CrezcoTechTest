using Crezco.Infrastructure.Persistence;

namespace Crezco.Location.Tests.Builders;

public class LocationDataBuilder
{
    private readonly string _ipAddress;
    private readonly LocationApiDataBuilder _apiBuilder;

    public LocationDataBuilder(string ipAddress, LocationApiDataBuilder apiBuilder)
    {
        _ipAddress = ipAddress;
        _apiBuilder = apiBuilder;
    }

    public LocationData Build()
        => new()
        {
            IpAddress = _ipAddress,
            CreatedAt = DateTime.UtcNow,
            Data = _apiBuilder.Build()
        };
}