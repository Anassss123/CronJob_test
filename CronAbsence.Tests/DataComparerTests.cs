using CronAbsence.Domain.Interfaces;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Process;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CronAbsence.Tests
{
    [TestFixture]
    public class DataComparerTests
    {
        private Mock<IPilotageRepository> _mockPilotageRepository;
        private DataComparer _dataComparer;

        [SetUp]
        public void SetUp()
        {
            _mockPilotageRepository = new Mock<IPilotageRepository>();
            _dataComparer = new DataComparer(_mockPilotageRepository.Object);
        }

        [Test]
        public async Task InsertNewAbsence_ShouldInsertNewRecords()
        {
            // Arrange
            var dbAbsence = new List<CatAbsence>
            {
                new CatAbsence { Matricule = 123, Date = new DateTime(2023, 1, 1), Motif = "Illness" }
            };
            var fileAbsence = new List<CatAbsence>
            {
                new CatAbsence { Matricule = 123, Date = new DateTime(2023, 1, 1), Motif = "Illness" },
                new CatAbsence { Matricule = 124, Date = new DateTime(2023, 1, 2), Motif = "Vacation" }
            };

            // Act
            await _dataComparer.InsertNewAbsence(dbAbsence, fileAbsence);

            // Assert
            _mockPilotageRepository.Verify(
                repo => repo.InsertCatAbsencesAsync(
                    It.Is<List<CatAbsence>>(list => list.Count == 1 && list[0].Matricule == 124)),
                Times.Once);
        }

        [Test]
        public async Task UpdateExistingAbsence_ShouldUpdateExistingRecords()
        {
            // Arrange
            var dbAbsence = new List<CatAbsence>
            {
                new CatAbsence { Matricule = 123, Date = new DateTime(2023, 1, 1), Motif = "Illness", Type = 1 }
            };
            var fileAbsence = new List<CatAbsence>
            {
                new CatAbsence { Matricule = 123, Date = new DateTime(2023, 1, 1), Motif = "Illness", Type = 2 }
            };

            // Act
            await _dataComparer.UpdateExistingAbsence(dbAbsence, fileAbsence);

            // Assert
            _mockPilotageRepository.Verify(
                repo => repo.UpdateCatAbsencesAsync(
                    It.Is<List<CatAbsence>>(list => list.Count == 1 && list[0].Type == 2)),
                Times.Once);
        }

        [Test]
        public async Task CancelDeletedAbsence_ShouldCancelDeletedRecords()
        {
            // Arrange
            var dbAbsence = new List<CatAbsence>
            {
                new CatAbsence { Matricule = 123, Date = new DateTime(2023, 1, 1), Motif = "Illness" }
            };
            var fileAbsence = new List<CatAbsence>
            {
                new CatAbsence { Matricule = 124, Date = new DateTime(2023, 1, 2), Motif = "Vacation" }
            };

            // Act
            await _dataComparer.CancelDeletedAbsence(dbAbsence, fileAbsence);

            // Assert
            _mockPilotageRepository.Verify(
                repo => repo.UpdateCatAbsencesAsync(
                    It.Is<List<CatAbsence>>(list => list.Count == 1 && list[0].Matricule == 123 && list[0].Type == 0)),
                Times.Once);
        }
    }
}
