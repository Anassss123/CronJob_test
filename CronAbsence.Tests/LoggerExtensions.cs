using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;

namespace CronAbsence.Tests
{
    public static class LoggerExtensions
    {
        public static List<(LogLevel logLevel, string message, Exception? exception)> GetLoggedMessages<T>(this Mock<ILogger<T>> mockLogger)
        {
            var loggedMessages = new List<(LogLevel logLevel, string message, Exception? exception)>();

            mockLogger
                .Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Callback((LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter) =>
                {
                    loggedMessages.Add((logLevel, formatter(state, exception), exception));
                });

            return loggedMessages;
        }
    }
}
