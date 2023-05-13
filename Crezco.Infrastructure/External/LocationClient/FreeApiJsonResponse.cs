namespace Crezco.Infrastructure.External.LocationClient;

/// <summary>
/// Matches the response found at: https://freeipapi.com/
/// </summary>
internal class FreeApiJsonResponse 
{
    public required int IPVersion { get; init; }
    public required string IPAddress { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required string CountryName { get; init; }
    public required string CountryCode { get; init; }
    public required string TimeZone { get; init; }
    public required string ZipCode { get; init; }
    public required string CityName { get; init; }
    public required string RegionName { get; init; }
}