using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Tests.Builders;
using FluentAssertions;

namespace Crezco.Infrastructure.Tests;

public class GetLocationCacheTests: IntegrationTests
{
    public ICache<LocationData> GetLocationCache() =>
        GetService<ICache<LocationData>>();

    private async Task<LocationData?> TryGetOrCreate(string ip, LocationData toInsertIfMissing)
    {
        ICache<LocationData> cache = GetLocationCache();
        return await cache.TryGetOrCreateAsync(ip, () => Task.FromResult(toInsertIfMissing)!);
    }

    [Fact]
    public async Task TryGetOrCreateAsync_NoPreviousValue_ReturnsValue()
    {
        // Arrange
        string ip = "1.2.3.4";
        LocationData toInsertIfMissing = new LocationBuilder(ip)
            .Build();

        // Act
        LocationData? result = await TryGetOrCreate(ip, toInsertIfMissing);

        // Assert
        result.Should().BeEquivalentTo(toInsertIfMissing);
    }

    [Fact]
    public async Task TryGetOrCreateAsync_WithPreviousValue_ReturnsPreviousValue()
    {
        // Arrange
        string ip = "1.2.3.4";
        LocationData firstInsert = new LocationBuilder(ip)
            .WithCityName("Tokyo")
            .Build();
        LocationData toInsertIfMissing = new LocationBuilder(ip)
            .WithCityName("London")
            .Build();

        // Act
        LocationData? result = await TryGetOrCreate(ip, firstInsert);
        result = await TryGetOrCreate(ip, toInsertIfMissing);

        // Assert
        result.Should().BeEquivalentTo(firstInsert);
        result.Data.CityName.Should().Be("Tokyo");
    }
    
}