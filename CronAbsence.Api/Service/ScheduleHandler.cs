using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CronAbsence.Infrastructure.Excel;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly ILogger<ScheduleHandler> _logger;
        private readonly IExcelReaderService _excelReaderService;

        public ScheduleHandler(ILogger<ScheduleHandler> logger, IExcelReaderService excelReaderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _excelReaderService = excelReaderService ?? throw new ArgumentNullException(nameof(excelReaderService));
        }

        public async Task ProcessAsync()
        {
            try
            {
                _logger.LogInformation("ProcessAsync is starting.");

                // // Specify the path to your .xls file
                string filePath = "C:\\Users\\Anas.HAMRAOUI\\Downloads\\TestFiles";

                // // Open the file using FileInfo
                FileInfo fileInfo = new FileInfo(filePath);

                // // Use the Excel reader service to read data from the file
                var data = await _excelReaderService.ReadDataAsync(fileInfo).ConfigureAwait(false);

                // // Display the extracted data
                var dataTable = _excelReaderService.ReadData(fileInfo);
                _excelReaderService.DisplayDataAsync(dataTable);

                // // Process the extracted data as needed
                


                _logger.LogInformation("ProcessAsync completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ProcessAsync.");
            }
        }
        
    }
}