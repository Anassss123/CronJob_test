using System;
using System.Collections.Generic;
using CronAbsence.Api.Service;
using CronAbsence.Domain.Models;
using CronAbsence.Api.Schedule.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CronAbsence.Tests
{
    public class LoggerServiceTests
    {
        private Mock<ILogger<LoggerService>> _mockLogger;
        private LoggerService _loggerService;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<LoggerService>>();
            _loggerService = new LoggerService(_mockLogger.Object);
        }

        [Test]
        public void LogCatAbsences_ShouldLogCorrectInformation()
        {
            // Arrange
            var catAbsences = new List<CatAbsence>
            {
                new CatAbsence
                {
                    Matricule = 10313,
                    Nom = "Ben Mansour",
                    Prenom = "Mohamed",
                    Date = DateTime.Parse("2024-05-23"),
                    Type = 1,
                    Debut = "", // Assuming these are strings, adjust as necessary
                    Fin = "",   // Assuming these are strings, adjust as necessary
                    Motif = "REPOS ASTREINTE",
                    Flag = 0
                },
            };

            // Act
            _loggerService.LogCatAbsences(catAbsences);

            // Assert
            foreach (var absence in catAbsences)
            {
                _mockLogger.Verify(
                    logger => logger.Log(
                        It.Is<LogLevel>(l => l == LogLevel.Information),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Matricule: {absence.Matricule}")),
                        null,
                        It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                    Times.Once);
            }
        }
    }
}
