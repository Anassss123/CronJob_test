using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Microsoft.Data.SqlClient;
using CronAbsence.Domain.Models;
using CronAbsence.Domain.Interfaces;
using CronAbsence.Infrastructure.Service.Data;
using Microsoft.Extensions.Configuration;

namespace CronAbsence.Tests
{
    [TestFixture]
    public class PilotageRepositoryTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<IDbConnection> _dbConnectionMock;
        private IPilotageRepository _pilotageRepository;

        [SetUp]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c.GetConnectionString("Default"))
                .Returns("fake_connection_string");

            _dbConnectionMock = new Mock<IDbConnection>();

            _pilotageRepository = new PilotageRepository(_configurationMock.Object)
            {
                DbConnection = _dbConnectionMock.Object
            };
        }

        [Test]
        public async Task GetCatAbsencesAsync_ReturnsExpectedResults()
        {
            // Arrange
            var expectedAbsences = new List<CatAbsence>
            {
                new CatAbsence
                {
                    ID = 1,
                    Matricule = 123,
                    Nom = "Doe",
                    Prenom = "John",
                    Date = DateTime.Now,
                    Type = 1,
                    Debut = "08:00",
                    Fin = "17:00",
                    Motif = "Illness",
                    Flag = 1,
                    LastUpdated = DateTime.Now
                }
            };

            _dbConnectionMock.Setup(db => db.QueryAsync<CatAbsence>(
                "[dbo].[ps_cat_absence_s_by_date]",
                null,
                null,
                null,
                CommandType.StoredProcedure)).ReturnsAsync(expectedAbsences);

            // Act
            var result = await _pilotageRepository.GetCatAbsencesAsync();

            // Assert
            ClassicAssert.AreEqual(expectedAbsences, result);
        }

        [Test]
        public async Task InsertCatAbsencesAsync_WithValidData_InsertsAbsences()
        {
            // Arrange
            var newAbsences = new List<CatAbsence>
            {
                new CatAbsence
                {
                    ID = 2,
                    Matricule = 456,
                    Nom = "Smith",
                    Prenom = "Jane",
                    Date = DateTime.Now,
                    Type = 2,
                    Debut = "08:00",
                    Fin = "17:00",
                    Motif = "Holiday",
                    Flag = 1,
                    LastUpdated = DateTime.Now
                }
            };

            _dbConnectionMock.Setup(db => db.ExecuteAsync(
                "[dbo].[ps_cat_absence_i]",
                It.IsAny<object>(),
                null,
                null,
                CommandType.StoredProcedure)).ReturnsAsync(1);

            // Act
            await _pilotageRepository.InsertCatAbsencesAsync(newAbsences);

            // Assert
            _dbConnectionMock.Verify(db => db.ExecuteAsync(
                "[dbo].[ps_cat_absence_i]",
                It.IsAny<object>(),
                null,
                null,
                CommandType.StoredProcedure), Times.Once);
        }

        [Test]
        public async Task InsertCatAbsencesAsync_WithEmptyList_DoesNotInsert()
        {
            // Arrange
            var newAbsences = new List<CatAbsence>();

            // Act
            await _pilotageRepository.InsertCatAbsencesAsync(newAbsences);

            // Assert
            _dbConnectionMock.Verify(db => db.ExecuteAsync(
                "[dbo].[ps_cat_absence_i]",
                It.IsAny<object>(),
                null,
                null,
                CommandType.StoredProcedure), Times.Never);
        }

        [Test]
        public async Task UpdateCatAbsencesAsync_WithValidData_UpdatesAbsences()
        {
            // Arrange
            var updatedAbsences = new List<CatAbsence>
            {
                new CatAbsence
                {
                    ID = 3,
                    Matricule = 789,
                    Nom = "Brown",
                    Prenom = "Alice",
                    Date = DateTime.Now,
                    Type = 3,
                    Debut = "08:00",
                    Fin = "17:00",
                    Motif = "Work from home",
                    Flag = 1,
                    LastUpdated = DateTime.Now
                }
            };

            _dbConnectionMock.Setup(db => db.ExecuteAsync(
                "[dbo].[ps_cat_absence_u]",
                It.IsAny<object>(),
                null,
                null,
                CommandType.StoredProcedure)).ReturnsAsync(1);

            // Act
            await _pilotageRepository.UpdateCatAbsencesAsync(updatedAbsences);

            // Assert
            _dbConnectionMock.Verify(db => db.ExecuteAsync(
                "[dbo].[ps_cat_absence_u]",
                It.IsAny<object>(),
                null,
                null,
                CommandType.StoredProcedure), Times.Once);
        }

        [Test]
        public void GetMaxIdFromDatabase_ReturnsExpectedMaxId()
        {
            // Arrange
            int expectedMaxId = 10;

            _dbConnectionMock.Setup(db => db.ExecuteScalar<int>(
                "SELECT MAX(ID) FROM Cat_absence",
                null,
                null,
                null,
                CommandType.Text)).Returns(expectedMaxId);

            // Act
            var result = _pilotageRepository.GetMaxIdFromDatabase();

            // Assert
            ClassicAssert.AreEqual(expectedMaxId, result);
        }
    }
}
