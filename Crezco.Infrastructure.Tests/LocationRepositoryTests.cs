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

    private static void AssertResult(Location? actual, Location expected)
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
        Location? result = await repository.GetLatestAsync(ip);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_StoresValue()
    {
        // Arrange
        LocationRepository repository = GetRepository();
        string ip = "1.2.3.4";
        Location toInsert = new LocationBuilder(ip)
            .Build();

        // Act
        await repository.CreateAsync(toInsert);
        Location? result = await repository.GetLatestAsync(ip);

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
        Location toInsert1 = new LocationBuilder(ip1)
            .Build();
        Location toInsert2 = new LocationBuilder(ip2)
            .Build();

        // Act
        await repository.CreateAsync(toInsert1);
        await repository.CreateAsync(toInsert2);

        Location? result1 = await repository.GetLatestAsync(ip1);
        Location? result2 = await repository.GetLatestAsync(ip2);

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

        Location toInsert1 = new LocationBuilder(ip)
            .WithCityName("Tokyo")
            .WithCreatedAt(DateTime.UtcNow.AddHours(-5))
            .Build();

        // expected to be returned
        Location toInsert2 = new LocationBuilder(ip)
            .WithCityName("London")
            .WithCreatedAt(DateTime.UtcNow.AddHours(-3))
            .Build();

        Location toInsert3 = new LocationBuilder(ip)
            .WithCityName("New York")
            .WithCreatedAt(DateTime.UtcNow.AddHours(-4))
            .Build();

        // Act
        await repository.CreateAsync(toInsert1);
        await repository.CreateAsync(toInsert2);
        await repository.CreateAsync(toInsert3);

        Location? result = await repository.GetLatestAsync(ip);

        // Assert
        AssertResult(result, toInsert2);
    }
}
