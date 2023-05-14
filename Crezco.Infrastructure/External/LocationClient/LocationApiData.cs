using ProtoBuf;

namespace Crezco.Infrastructure.External.LocationClient;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public sealed class LocationApiData
{
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required string CountryName { get; init; }
    public required string CountryCode { get; init; }
    public required string TimeZone { get; init; }
    public required string ZipCode { get; init; }
    public required string CityName { get; init; }
    public required string RegionName { get; init; }
}