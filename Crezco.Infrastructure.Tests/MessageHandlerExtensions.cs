using FakeItEasy;
using FakeItEasy.Configuration;

namespace Crezco.Infrastructure.Tests;

public static class MessageHandlerExtensions
{
    public static IAnyCallConfigurationWithNoReturnTypeSpecified WhereSendAsync(this HttpMessageHandler messageHandler) =>
        A.CallTo(messageHandler).Where(call => call.Method.Name == "SendAsync");
}