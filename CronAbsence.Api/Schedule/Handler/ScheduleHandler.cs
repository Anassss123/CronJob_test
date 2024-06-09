using System.Data;
using System.Data.SqlClient;
using System.Net;
using CronAbsence.Domain.Interfaces;
using CronAbsence.Domain.Service;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Infrastructure.Service.Process;
using Dapper;
using CronAbsence.Infrastructure.Interfaces;
using CronAbsence.Api.Schedule.Interface;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IDataComparer _dataComparer;
        private readonly IDatabaseReaderService _databaseReaderService;
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IFtpService _ftpService;
        private readonly ILoggerService _loggerService;
        private readonly IDataConverter _dataConverter;
        
        public ScheduleHandler(IExcelReaderService excelReaderService, IDataComparer dataComparer, IDatabaseReaderService databaseReaderService, IConfiguration configuration, IFtpService ftpService, ILoggerService loggerService, IDataConverter dataConverter)
        {
            _excelReaderService = excelReaderService;
            _dataComparer = dataComparer;
            _databaseReaderService = databaseReaderService;
            _connectionString = configuration.GetConnectionString("Default");
            _configuration = configuration;
             _ftpService = ftpService;
            _loggerService = loggerService;
            _dataConverter = dataConverter;
        }

        public async Task ProcessAsync()
        {
            try
            {
                Console.WriteLine("Processing started...");

                // FTP server details
                string ftpHost = _configuration["FTPServer:Host"]; // localhost
                string ftpPort = _configuration["FTPServer:Port"]; // 21
                string ftpUserName = _configuration["FTPServer:User"]; // user01
                string ftpPassword = _configuration["FTPServer:Password"]; // 1234
                string fileName = "dummyFile.csv";

                // File paths
                string localDownloadPath = Path.Combine(Path.GetTempPath(), fileName); // Temp folder for downloading
                string localArchivePath = Path.Combine(_configuration["FTPServer:DestinationFolderPath"], fileName); // Archive folder after processing
                string localOriginalPath = Path.Combine(_configuration["FTPServer:SourceFolderPath"], fileName); // Original folder before processing

                await _ftpService.DownloadFileAsync(ftpHost, ftpPort, ftpUserName, ftpPassword, fileName, localDownloadPath);

                var csvData = await _excelReaderService.ReadDataAsync(localDownloadPath);

                var fileAbsences = _dataConverter.ConvertDataTableToCatAbsenceList(csvData);

                _loggerService.LogCatAbsences(fileAbsences);

                // Get data from the database
                var dbAbsence = await _databaseReaderService.GetCatAbsencesAsync();

                // Compare data and perform operations
                await _dataComparer.InsertNewAbsence(dbAbsence, fileAbsences);
                await _dataComparer.UpdateExistingAbsence(dbAbsence, fileAbsences);
                await _dataComparer.CancelDeletedAbsence(dbAbsence, fileAbsences);

                // Move the file to the archive folder
                File.Move(localOriginalPath, localArchivePath);

                // Call Api PPM 
                

                Console.WriteLine("Processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
