using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CronAbsence.Infrastructure.Service.Data;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Process;
using Serilog;
using System.Data.Common;
using System.Text;
using static CronAbsence.Infrastructure.Service.Data.DatabaseReaderService;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly ILogger<ScheduleHandler> _logger;
        private readonly IExcelReaderService _excelReaderService;
        private readonly IDatabaseReaderService _databaseReaderService;
        // private readonly IDataComparer _dataComparer;
        // private readonly IDataProcessor _dataProcessor;

         public ScheduleHandler(ILogger<ScheduleHandler> logger, IDatabaseReaderService databaseReaderService, IExcelReaderService excelReaderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _excelReaderService = excelReaderService ?? throw new ArgumentNullException(nameof(excelReaderService));
            _databaseReaderService = databaseReaderService ?? throw new ArgumentNullException(nameof(databaseReaderService));
            // _dataComparer = dataComparer ?? throw new ArgumentNullException(nameof(dataComparer));
            // _dataProcessor = dataProcessor ?? throw new ArgumentNullException(nameof(dataProcessor));
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

                // //Display the extracted data
                var dataTable = _excelReaderService.ReadData(fileInfo);
                _excelReaderService.DisplayDataAsync(dataTable);

                // // Extract data from the database
                // var dbCatAbsences = await _databaseReaderService.GetCatAbsencesAsync();
                var dbCatAbsenceStatuts = await _databaseReaderService.GetCatAbsenceStatutsAsync();
                var catAbsenceTable = await _databaseReaderService.GetCatAbsencesTableAsync();

                LogDatabaseData(catAbsenceTable);

                // // Compare data from the database with data from Excel
                // var updatedData = CompareData(dbCatAbsences, data);

                // // Process the updated data as needed
                // ProcessUpdatedData(updatedData);

                _logger.LogInformation("ProcessAsync completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ProcessAsync.");
            }
        }
        private void LogDatabaseData(DatabaseReaderService.CatAbsenceTable catAbsenceTable)
        {
            _logger.LogInformation("Logging CatAbsence database data:");

            foreach (var catAbsence in catAbsenceTable.Absences)
            {
                _logger.LogInformation($"Id: {catAbsence.Id}, Matricule: {catAbsence.Matricule}, Nom: {catAbsence.Nom}, Prenom: {catAbsence.Prenom}, DateAbsence: {catAbsence.DateAbsence}, AbsenceStatutId: {catAbsence.AbsenceStatutId}, LastUpdate: {catAbsence.LastUpdate}, UpdateFlag: {catAbsence.UpdateFlag}, Type: {catAbsence.Type}");
            }
        }

    }
}
