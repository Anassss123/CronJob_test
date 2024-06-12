using System;
using System.Threading;
using System.Threading.Tasks;
using CronAbsence.Api.Schedule;
using CronAbsence.Api.Schedule.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace CronAbsence.Tests
{
    public class ScheduleJobServiceTests
    {
        private Mock<ILogger<ScheduleJobService>> _mockLogger;
        private Mock<IServiceScopeFactory> _mockServiceScopeFactory;
        private ScheduleJobService _scheduleJobService;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ScheduleJobService>>();
            _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            _scheduleJobService = new ScheduleJobService(_mockLogger.Object, _mockServiceScopeFactory.Object);
        }

        [Test]
        public async Task ExecuteAsync_ShouldProcessScheduleHandler()
        {
            // Arrange
            var mockScope = new Mock<IServiceScope>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockScheduleHandler = new Mock<IScheduleHandler>();

            _mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockScope.Object);
            mockScope.Setup(scope => scope.ServiceProvider).Returns(mockServiceProvider.Object);
            mockServiceProvider.Setup(provider => provider.GetService(typeof(IScheduleHandler))).Returns(mockScheduleHandler.Object);

            // Act
            await _scheduleJobService.StartAsync(CancellationToken.None);
            await _scheduleJobService.StopAsync(CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService is starting."), Times.Once);
            _mockServiceScopeFactory.Verify(factory => factory.CreateScope(), Times.Once);
            mockServiceProvider.Verify(provider => provider.GetService(typeof(IScheduleHandler)), Times.Once);
            mockScheduleHandler.Verify(handler => handler.ProcessAsync(), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService completed successfully."), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService is stopping."), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_ShouldLogErrorWhenExceptionOccurs()
        {
            // Arrange
            var mockScope = new Mock<IServiceScope>();

            _mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockScope.Object);
            mockScope.Setup(scope => scope.ServiceProvider).Throws(new Exception("Test Exception"));

            var loggedMessages = _mockLogger.GetLoggedMessages();

            // Act
            await _scheduleJobService.StartAsync(CancellationToken.None);
            await _scheduleJobService.StopAsync(CancellationToken.None);

            // Assert
            Assert.IsTrue(loggedMessages.Exists(m => m.logLevel == LogLevel.Error && m.message.Contains("Error occurred in ScheduleJobService.") && m.exception != null));
            _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService is stopping."), Times.Once);
        }
    }
}
