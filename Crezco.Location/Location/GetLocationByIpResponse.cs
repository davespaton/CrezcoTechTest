namespace Crezco.Location.Location;

public sealed class GetLocationByIpResponse
{
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required string CountryName { get; init; }
    public required string CityName { get; init; }
    public required string RegionName { get; init; }
}