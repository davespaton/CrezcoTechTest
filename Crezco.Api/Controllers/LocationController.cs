using Microsoft.AspNetCore.Mvc;
using System.Net;
using Crezco.Location.Location;

namespace Crezco.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("{ipAddress}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetLocationByIpResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(string ipAddress, CancellationToken cancellationToken)
    {
        IPAddress.TryParse(ipAddress, out IPAddress? ip);
        if (ip is null)
        {
            return BadRequest(new Dictionary<string, string>()
            {
                {nameof(ipAddress), "IP Address is in an invalid format"}
            });
        }

        GetLocationByIpResponse? response = await _locationService.GetLocationByIpAsync(ip, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}

