using System.Net;
using Crezco.Infrastructure.External.LocationClient;
using Crezco.Infrastructure.Tests.Extensions;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Crezco.Infrastructure.Tests;

public class FreeApiClientUnitTests
{
    private readonly IHttpClientFactory _clientFactory = A.Fake<IHttpClientFactory>();
    private readonly ILogger<FreeApiClient> _logger = A.Fake<ILogger<FreeApiClient>>();

    private FreeApiClient GetFreeApiClient() =>
        new(_clientFactory, _logger);

    [Fact]
    public async Task FreeApiClient_LogsError_IfApiThrowsException()
    {
        // Arrange
        var messageHandler = A.Fake<HttpMessageHandler>();
        messageHandler.WhereSendAsync()
            .Throws<HttpRequestException>();

        HttpClient httpClient = new HttpClient(messageHandler);

        FreeApiClient client = GetFreeApiClient();
        A.CallTo(() => _clientFactory.CreateClient(FreeApiClient.HttpClientName))
            .Returns(httpClient);

        // Act
        LocationApiData? result = await client.GetLocationAsync(IPAddress.Parse("1.1.1.1"));

        // Assert
        _logger.AssertLog(LogLevel.Error)
            .MustHaveHappenedOnceExactly();
        Assert.Null(result);
    }
}