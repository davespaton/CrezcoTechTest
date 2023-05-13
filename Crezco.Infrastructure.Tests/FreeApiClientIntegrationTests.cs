using System.Net;
using Crezco.Infrastructure.External.LocationClient;
using FluentAssertions;

namespace Crezco.Infrastructure.Tests;

public class FreeApiClientIntegrationTests : IntegrationTests
{
    [Fact]
    public async Task FreeApiClient_ReturnsLocationData_IfApiReturnsData()
    {
        var expectedData = new LocationApiData()
        {
            CityName = "Mountain View",
            CountryCode = "US",
            CountryName = "United States of America",
            Latitude = 37.405991,
            Longitude = -122.078514,
            RegionName = "California",
            TimeZone = "-07:00",
            ZipCode = "94043"
        };

        // Arrange
        ILocationClient client = GetService<ILocationClient>();

        // Act
        LocationApiData? result = await client.GetLocationAsync(IPAddress.Parse("8.8.8.8"));

        // Assert
        result.Should().BeEquivalentTo(expectedData);
    }
}