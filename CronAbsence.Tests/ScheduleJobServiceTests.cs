// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using CronAbsence.Api.Schedule;
// using CronAbsence.Api.Schedule.Interface;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NUnit.Framework;

// namespace CronAbsence.Tests
// {
//     public class ScheduleJobServiceTests
//     {
//         private Mock<ILogger<ScheduleJobService>> _mockLogger;
//         private Mock<IServiceScopeFactory> _mockServiceScopeFactory;
//         private ScheduleJobService _scheduleJobService;

//         [SetUp]
//         public void Setup()
//         {
//             _mockLogger = new Mock<ILogger<ScheduleJobService>>();
//             _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
//             _scheduleJobService = new ScheduleJobService(_mockLogger.Object, _mockServiceScopeFactory.Object);
//         }

//         [TearDown]
//         public void TearDown()
//         {
//             _scheduleJobService.Dispose();
//         }

//         [Test]
//         public async Task ExecuteAsync_ShouldProcessScheduleHandler()
//         {
//             // Arrange
//             var mockScope = new Mock<IServiceScope>();
//             var mockServiceProvider = new Mock<IServiceProvider>();
//             var mockScheduleHandler = new Mock<IScheduleHandler>();

//             _mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockScope.Object);
//             mockScope.Setup(scope => scope.ServiceProvider).Returns(mockServiceProvider.Object);
//             mockServiceProvider.Setup(provider => provider.GetService(typeof(IScheduleHandler))).Returns(mockScheduleHandler.Object);

//             // Setup the ProcessAsync method
//             mockScheduleHandler.Setup(handler => handler.ProcessAsync()).Returns(Task.CompletedTask);

//             // Act
//             await _scheduleJobService.StartAsync(CancellationToken.None);
//             await _scheduleJobService.StopAsync(CancellationToken.None);

//             // Assert
//             _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService is starting."), Times.Once);
//             _mockServiceScopeFactory.Verify(factory => factory.CreateScope(), Times.Once);
//             mockServiceProvider.Verify(provider => provider.GetService(typeof(IScheduleHandler)), Times.Once);
//             mockScheduleHandler.Verify(handler => handler.ProcessAsync(), Times.Once);
//             _mockLogger.Verify(logger => logger.LogInformation(It.IsAny<string>()), Times.Once);
//             _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService is stopping."), Times.Once);
//         }

//         [Test]
//         public async Task ExecuteAsync_ShouldLogErrorWhenExceptionOccurs()
//         {
//             // Arrange
//             var mockScope = new Mock<IServiceScope>();

//             _mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockScope.Object);
//             mockScope.Setup(scope => scope.ServiceProvider).Throws(new Exception("Test Exception"));

//             // Act
//             await _scheduleJobService.StartAsync(CancellationToken.None);
//             await _scheduleJobService.StopAsync(CancellationToken.None);

//             // Assert
//             _mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
//             _mockLogger.Verify(logger => logger.LogInformation("ScheduleJobService is stopping."), Times.Once);
//         }
//     }
// }
