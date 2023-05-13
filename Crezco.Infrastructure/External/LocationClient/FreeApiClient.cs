using System.Net;
using System.Net.Http.Json;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Crezco.Infrastructure.External.LocationClient;

public class FreeApiClient : ILocationClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FreeApiClient> _logger;

    public const string HttpClientName = "FreeApiClient";

    public FreeApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<FreeApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<LocationApiData?> GetLocationAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
    {
        using HttpClient client = _httpClientFactory.CreateClient(HttpClientName);

        string ip = ipAddress.ToString();

        try
        {
            var response = await client.GetFromJsonAsync<FreeApiJsonResponse>($"json/{ip}", cancellationToken);
            return response?.Adapt<LocationApiData>();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failure to get location for {ip}", ip);
            return null;
        }
    }
}