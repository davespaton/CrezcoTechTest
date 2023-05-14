using System.Net;
using Crezco.Api.Controllers;
using Crezco.Location.Location;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Crezco.Api.Tests;
public class LocationControllerTests
{
    private readonly ILocationService _locationService = A.Fake<ILocationService>();
    LocationController GetController() => new LocationController(_locationService);

    private CancellationToken CancellationToken => CancellationToken.None;

    [Fact]
    public async Task LocationController_InvalidIp_ReturnsBadRequest()
    {
        // Arrange
        LocationController controller = GetController();

        // Act
        IActionResult result = await controller.Get("invalid ip", CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result;
        badRequest.Value.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "ipAddress", "IP Address is in an invalid format" }
        });
    }

    [Fact]
    public async Task LocationController_ValidIp_ReturnsLocationData()
    {
        // Arrange
        LocationController controller = GetController();
        var response = new GetLocationByIpResponse()
        {
            CityName = "Tokyo",
            CountryName = "Japan",
            Latitude = 1.1,
            Longitude = 2.2,
            RegionName = "Asia"
        };

        A.CallTo(() => _locationService.GetLocationByIpAsync(A<IPAddress>.Ignored, CancellationToken))!
            .Returns(Task.FromResult(response));

        // Act
        IActionResult result = await controller.Get("1.1.1.1", CancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task LocationController_IpNotFound_ReturnsNotFound()
    {
        // Arrange
        LocationController controller = GetController();
        A.CallTo(() => _locationService.GetLocationByIpAsync(A<IPAddress>.Ignored, CancellationToken))!
            .Returns(Task.FromResult<GetLocationByIpResponse?>(null));

        // Act
        IActionResult result = await controller.Get("1.1.1.1", CancellationToken);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
