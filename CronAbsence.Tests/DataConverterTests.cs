using System;
using System.Data;
using CronAbsence.Domain.Interfaces;
using CronAbsence.Domain.Service;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy; // Added for ClassicAssert

namespace CronAbsence.Tests
{
    [TestFixture]
    public class DataConverterTests
    {
        private Mock<IPilotageRepository> _mockPilotageRepository;
        private DataConverter _dataConverter;

        [SetUp]
        public void SetUp()
        {
            _mockPilotageRepository = new Mock<IPilotageRepository>();
            _dataConverter = new DataConverter(_mockPilotageRepository.Object);
        }

        [Test]
        public void ConvertDataTableToCatAbsenceList_ShouldConvertDataCorrectly()
        {
            // Arrange
            _mockPilotageRepository.Setup(repo => repo.GetMaxIdFromDatabase()).Returns(100);

            var dataTable = new DataTable();
            dataTable.Columns.Add("Matricule");
            dataTable.Columns.Add("Nom");
            dataTable.Columns.Add("Prenom");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Nombre Heures");
            dataTable.Columns.Add("Debut");
            dataTable.Columns.Add("Fin");
            dataTable.Columns.Add("Motif");

            dataTable.Rows.Add("123", "Doe", "John", "01/01/2023", "J8", "08:00", "16:00", "ABSENCE");
            dataTable.Rows.Add("124", "Smith", "Jane", "02/01/2023", "M4", "08:00", "12:00", "REPOS ASTREINTE");

            // Act
            var result = _dataConverter.ConvertDataTableToCatAbsenceList(dataTable);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
            ClassicAssert.AreEqual(101, result[0].ID);
            ClassicAssert.AreEqual(123, result[0].Matricule);
            ClassicAssert.AreEqual("Doe", result[0].Nom);
            ClassicAssert.AreEqual("John", result[0].Prenom);
            ClassicAssert.AreEqual(new DateTime(2023, 1, 1), result[0].Date);
            ClassicAssert.AreEqual(3, result[0].Type); // "J8" -> 3
            ClassicAssert.AreEqual("08:00", result[0].Debut);
            ClassicAssert.AreEqual("16:00", result[0].Fin);
            ClassicAssert.AreEqual("ABSENCE", result[0].Motif);
            ClassicAssert.AreEqual(0, result[0].Flag);

            ClassicAssert.AreEqual(102, result[1].ID);
            ClassicAssert.AreEqual(124, result[1].Matricule);
            ClassicAssert.AreEqual("Smith", result[1].Nom);
            ClassicAssert.AreEqual("Jane", result[1].Prenom);
            ClassicAssert.AreEqual(new DateTime(2023, 1, 2), result[1].Date);
            ClassicAssert.AreEqual(1, result[1].Type); // "M4" -> 1
            ClassicAssert.AreEqual("08:00", result[1].Debut);
            ClassicAssert.AreEqual("12:00", result[1].Fin);
            ClassicAssert.AreEqual("REPOS ASTREINTE", result[1].Motif);
            ClassicAssert.AreEqual(0, result[1].Flag);
        }

        [Test]
        public void ConvertDataTableToCatAbsenceList_ShouldThrowExceptionForInvalidType()
        {
            // Arrange
            _mockPilotageRepository.Setup(repo => repo.GetMaxIdFromDatabase()).Returns(100);

            var dataTable = new DataTable();
            dataTable.Columns.Add("Matricule");
            dataTable.Columns.Add("Nom");
            dataTable.Columns.Add("Prenom");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Nombre Heures");
            dataTable.Columns.Add("Debut");
            dataTable.Columns.Add("Fin");
            dataTable.Columns.Add("Motif");

            dataTable.Rows.Add("123", "Doe", "John", "01/01/2023", "X8", "08:00", "16:00", "ABSENCE");

            // Act & Assert
            ClassicAssert.Throws<ArgumentException>(() => _dataConverter.ConvertDataTableToCatAbsenceList(dataTable));
        }
    }
}
