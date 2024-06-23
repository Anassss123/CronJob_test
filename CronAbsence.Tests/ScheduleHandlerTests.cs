using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using CronAbsence.Api.Service;
using CronAbsence.Api.Schedule.Interface;
using CronAbsence.Domain.Interfaces;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Interfaces;
using Moq;
using NUnit.Framework;

namespace CronAbsence.Tests
{
    public class ScheduleHandlerTests
    {
        private Mock<IExcelReaderService> _mockExcelReaderService;
        private Mock<IDataComparer> _mockDataComparer;
        private Mock<IPilotageRepository> _mockPilotageRepository;
        private Mock<IFTPProvider> _mockFtpProvider;
        private Mock<ILoggerService> _mockLoggerService;
        private Mock<IDataConverter> _mockDataConverter;
        private ScheduleHandler _scheduleHandler;

        [SetUp]
        public void Setup()
        {
            _mockExcelReaderService = new Mock<IExcelReaderService>();
            _mockDataComparer = new Mock<IDataComparer>();
            _mockPilotageRepository = new Mock<IPilotageRepository>();
            _mockFtpProvider = new Mock<IFTPProvider>();
            _mockLoggerService = new Mock<ILoggerService>();
            _mockDataConverter = new Mock<IDataConverter>();
            _scheduleHandler = new ScheduleHandler(
                
                _mockExcelReaderService.Object,
                _mockDataComparer.Object,
                _mockPilotageRepository.Object,
                _mockFtpProvider.Object,
                _mockLoggerService.Object,
                _mockDataConverter.Object
            );
        }

        [Test]
        public async Task ProcessAsync_ShouldProcessSuccessfully()
        {
            // Arrange
            string fileName = "dummyFile.csv";
            string localDownloadPath = Path.Combine(Path.GetTempPath(), fileName);
            string localArchivePath = Path.Combine(Path.GetTempPath(), "Archive", fileName);
            string localOriginalPath = Path.Combine(Path.GetTempPath(), "Original", fileName);

            // Mock FTPProvider methods
            _mockFtpProvider.Setup(p => p.GetLocalDownloadPath(fileName)).Returns(localDownloadPath);
            _mockFtpProvider.Setup(p => p.GetLocalArchivePath(fileName)).Returns(localArchivePath);
            _mockFtpProvider.Setup(p => p.GetLocalOriginalPath(fileName)).Returns(localOriginalPath);
            _mockFtpProvider.Setup(p => p.DownloadFileAsync(fileName, localDownloadPath)).Returns(Task.CompletedTask);

            // Mock ExcelReaderService method
            var mockDataTable = new DataTable();
            _mockExcelReaderService.Setup(s => s.ReadDataAsync(localDownloadPath)).ReturnsAsync(mockDataTable);

            // Mock DataConverter method
            var mockCatAbsenceList = new List<CatAbsence>();
            _mockDataConverter.Setup(c => c.ConvertDataTableToCatAbsenceList(mockDataTable)).Returns(mockCatAbsenceList);

            // Mock PilotageRepository method
            _mockPilotageRepository.Setup(r => r.GetCatAbsencesAsync()).ReturnsAsync(mockCatAbsenceList);

            // Mock DataComparer methods
            _mockDataComparer.Setup(c => c.InsertNewAbsence(mockCatAbsenceList, mockCatAbsenceList)).Returns(Task.CompletedTask);
            _mockDataComparer.Setup(c => c.UpdateExistingAbsence(mockCatAbsenceList, mockCatAbsenceList)).Returns(Task.CompletedTask);
            _mockDataComparer.Setup(c => c.CancelDeletedAbsence(mockCatAbsenceList, mockCatAbsenceList)).Returns(Task.CompletedTask);

            // Act
            await _scheduleHandler.ProcessAsync();

            // Assert
            _mockFtpProvider.Verify(p => p.GetLocalDownloadPath(fileName), Times.Once);
            _mockFtpProvider.Verify(p => p.GetLocalArchivePath(fileName), Times.Once);
            _mockFtpProvider.Verify(p => p.GetLocalOriginalPath(fileName), Times.Once);
            _mockFtpProvider.Verify(p => p.DownloadFileAsync(fileName, localDownloadPath), Times.Once);
            _mockExcelReaderService.Verify(s => s.ReadDataAsync(localDownloadPath), Times.Once);
            _mockDataConverter.Verify(c => c.ConvertDataTableToCatAbsenceList(mockDataTable), Times.Once);
            _mockPilotageRepository.Verify(r => r.GetCatAbsencesAsync(), Times.Once);
            _mockDataComparer.Verify(c => c.InsertNewAbsence(mockCatAbsenceList, mockCatAbsenceList), Times.Once);
            _mockDataComparer.Verify(c => c.UpdateExistingAbsence(mockCatAbsenceList, mockCatAbsenceList), Times.Once);
            _mockDataComparer.Verify(c => c.CancelDeletedAbsence(mockCatAbsenceList, mockCatAbsenceList), Times.Once);
            _mockLoggerService.Verify(l => l.LogCatAbsences(mockCatAbsenceList), Times.Once);
        }
    }
}
