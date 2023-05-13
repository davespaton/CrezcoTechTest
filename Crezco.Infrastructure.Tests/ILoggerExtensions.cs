using FakeItEasy.Configuration;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Crezco.Infrastructure.Tests;
// ReSharper disable once InconsistentNaming
public static class ILoggerExtensions
{
    public static IAnyCallConfigurationWithNoReturnTypeSpecified AssertLog(this ILogger logger, LogLevel logLevel = LogLevel.Information) =>
        A.CallTo(logger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == logLevel);
}