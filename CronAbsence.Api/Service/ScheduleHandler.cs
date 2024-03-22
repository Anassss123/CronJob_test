using CronAbsence.Infrastructure.Service.Data;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Infrastructure.Service.Process;
using System;
using System.Threading.Tasks;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IDataComparer _dataComparer;

        public ScheduleHandler(IExcelReaderService excelReaderService, IDataComparer dataComparer)
        {
            _excelReaderService = excelReaderService;
            _dataComparer = dataComparer;
        }

        public async Task ProcessAsync()
        {
            try
            {
                Console.WriteLine("ProcessAsync is starting.");

                // FTP Configuration
                string ftpServer = "ftp.example.com";
                string ftpUsername = "username";
                string ftpPassword = "password";
                string remoteFilePath = "/path/to/excel.xlsx";
                string localFilePath = "C:/temp/excel.xlsx";

                // Read Excel data from FTP
                var excelData = await _excelReaderService.ReadDataAsync(ftpServer, ftpUsername, ftpPassword, remoteFilePath, localFilePath );

                // Compare Excel data with database data
                var updatedData = await _dataComparer.CompareDataAsync(excelData);

                // Send updated data to PPM API (implement this)

                Console.WriteLine("ProcessAsync completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in ProcessAsync: {ex.Message}");
            }
        }
    }
}
