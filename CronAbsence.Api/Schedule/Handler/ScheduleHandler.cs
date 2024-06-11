using CronAbsence.Domain.Interfaces;
using CronAbsence.Infrastructure.Interfaces;
using CronAbsence.Api.Schedule.Interface;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IDataComparer _dataComparer;
        private readonly IPilotageRepository _PilotageRepository;
        private readonly IFTPProvider _ftpProvider;
        private readonly ILoggerService _loggerService;
        private readonly IDataConverter _dataConverter;

        public ScheduleHandler(
            IExcelReaderService excelReaderService,
            IDataComparer dataComparer,
            IPilotageRepository PilotageRepository,
            IFTPProvider ftpProvider,
            ILoggerService loggerService,
            IDataConverter dataConverter)
        {
            _excelReaderService = excelReaderService;
            _dataComparer = dataComparer;
            _PilotageRepository = PilotageRepository;
            _ftpProvider = ftpProvider;
            _loggerService = loggerService;
            _dataConverter = dataConverter;
        }

        public async Task ProcessAsync()
        {
            try
            {
                Console.WriteLine("Processing started...");

                string fileName = "dummyFile.csv";

                // Use FTPProvider to get file paths
                string localDownloadPath = _ftpProvider.GetLocalDownloadPath(fileName);
                string localArchivePath = _ftpProvider.GetLocalArchivePath(fileName);
                string localOriginalPath = _ftpProvider.GetLocalOriginalPath(fileName);

                await _ftpProvider.DownloadFileAsync(fileName, localDownloadPath);

                var csvData = await _excelReaderService.ReadDataAsync(localDownloadPath);

                var fileAbsences = _dataConverter.ConvertDataTableToCatAbsenceList(csvData);

                _loggerService.LogCatAbsences(fileAbsences);

                // Get data from the database
                var dbAbsence = await _PilotageRepository.GetCatAbsencesAsync();

                // Compare data and perform operations
                await _dataComparer.InsertNewAbsence(dbAbsence, fileAbsences);
                await _dataComparer.UpdateExistingAbsence(dbAbsence, fileAbsences);
                await _dataComparer.CancelDeletedAbsence(dbAbsence, fileAbsences);

                // Move the file to the archive folder
                File.Move(localOriginalPath, localArchivePath);

                Console.WriteLine("Processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
