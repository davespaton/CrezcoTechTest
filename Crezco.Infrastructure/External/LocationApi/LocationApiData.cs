using ProtoBuf;

namespace Crezco.Infrastructure.External.LocationApi;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class LocationApiData
{
    public required string Latitude { get; init; }
    public required string Longitude { get; init; }
    public required string CountryName { get; init; }
    public required string CountryCode { get; init; }
    public required string TimeZone { get; init; }
    public required string ZipCode { get; init; }
    public required string CityName { get; init; }
    public required string RegionName { get; init; }
}