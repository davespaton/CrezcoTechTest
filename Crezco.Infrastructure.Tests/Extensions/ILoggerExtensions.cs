﻿using FakeItEasy.Configuration;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Crezco.Infrastructure.Tests.Extensions;
// ReSharper disable once InconsistentNaming
public static class ILoggerExtensions
{
    public static IAnyCallConfigurationWithNoReturnTypeSpecified AssertLog<T>(this ILogger<T> logger, LogLevel logLevel = LogLevel.Information) =>
        A.CallTo(logger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == logLevel);
}