using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Repositories;
using Crezco.Infrastructure.Tests.Builders;
using FluentAssertions;
using MongoDB.Driver;

namespace Crezco.Infrastructure.Tests;
public class LocationRepositoryTests: IntegrationTests
{
    private LocationRepository GetRepository() =>
        new LocationRepository(GetService<IMongoDatabase>());

    private static void AssertResult(LocationData? actual, LocationData expected)
    {
        actual.Should().BeEquivalentTo(expected, opt => opt
            .Excluding(x => x.CreatedAt));
        actual.CreatedAt.Should().BeCloseTo(expected.CreatedAt, TimeSpan.FromSeconds(0.001));
    }

    [Fact]
    public async Task GetLatestAsync_NoPreviousValue_ReturnsNull()
    {
        // Arrange
        string ip = "1.2.3.4";
        LocationRepository repository = GetRepository();

        // Act
        LocationData? result = await repository.GetLatestAsync(ip);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_StoresValue()
    {
        // Arrange
        LocationRepository repository = GetRepository();
        string ip = "1.2.3.4";
        LocationData toInsert = new LocationBuilder(ip)
            .Build();

        // Act
        await repository.CreateAsync(toInsert);
        LocationData? result = await repository.GetLatestAsync(ip);

        // Assert
        AssertResult(result, toInsert);
    }

    [Fact]
    public async Task CreateAsync_StoresMultipleValues()
    {
        // Arrange
        LocationRepository repository = GetRepository();
        string ip1 = "1.2.3.4";
        string ip2 = "2.3.4.5";
        LocationData toInsert1 = new LocationBuilder(ip1)
            .Build();
        LocationData toInsert2 = new LocationBuilder(ip2)
            .Build();

        // Act
        await repository.CreateAsync(toInsert1);
        await repository.CreateAsync(toInsert2);

        LocationData? result1 = await repository.GetLatestAsync(ip1);
        LocationData? result2 = await repository.GetLatestAsync(ip2);

        // Assert
        AssertResult(result1, toInsert1);
        AssertResult(result2, toInsert2);
    }

    [Fact]
    public async Task GetLatestAsync_SameIPAddress_ReturnsLatest()
    {
        // Arrange
        LocationRepository repository = GetRepository();
        string ip = "1.2.3.4";

        LocationData toInsert1 = new LocationBuilder(ip)
            .WithCityName("Tokyo")
            .WithCreatedAt(DateTime.UtcNow.AddHours(-5))
            .Build();

        // expected to be returned
        LocationData toInsert2 = new LocationBuilder(ip)
            .WithCityName("London")
            .WithCreatedAt(DateTime.UtcNow.AddHours(-3))
            .Build();

        LocationData toInsert3 = new LocationBuilder(ip)
            .WithCityName("New York")
            .WithCreatedAt(DateTime.UtcNow.AddHours(-4))
            .Build();

        // Act
        await repository.CreateAsync(toInsert1);
        await repository.CreateAsync(toInsert2);
        await repository.CreateAsync(toInsert3);

        LocationData? result = await repository.GetLatestAsync(ip);

        // Assert
        AssertResult(result, toInsert2);
    }
}
