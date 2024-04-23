using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Infrastructure.Service.Process;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IDataComparer _dataComparer;
        private readonly IDatabaseReaderService _databaseReaderService;

        public ScheduleHandler(IExcelReaderService excelReaderService, IDataComparer dataComparer, IDatabaseReaderService databaseReaderService)
        {
            _excelReaderService = excelReaderService;
            _dataComparer = dataComparer;
            _databaseReaderService = databaseReaderService;
        }

        public async Task ProcessAsync()
        {
            try
            {
                Console.WriteLine("Processing started...");

                // CSV file path
                string csvFilePath = "C:/Users/Anas.HAMRAOUI/Downloads/TestFiles/dummyFile.csv";

                // Read data from CSV file
                var csvData = await _excelReaderService.ReadDataAsync(csvFilePath);

                // Log the CSV data for verification
                Console.WriteLine("CSV Data:");
                LogDataTable(csvData);

                // Compare data and get updated/new records
                var updatedData = await _dataComparer.CompareDataAsync(csvData);

                // Log the updated data for verification
                Console.WriteLine("Updated Data:");
                LogCatAbsenceList(updatedData);

                // Update CatAbsences in the database
                await _databaseReaderService.UpdateCatAbsencesAsync(updatedData);

                Console.WriteLine("Processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void LogDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    Console.Write($"{col.ColumnName}: {row[col]} | ");
                }
                Console.WriteLine();
            }
        }

        private void LogCatAbsenceList(IEnumerable<CatAbsence> catAbsences)
        {
            foreach (var catAbsence in catAbsences)
            {
                Console.WriteLine($"Matricule: {catAbsence.Matricule}, " +
                                  $"Nom: {catAbsence.Nom}, " +
                                  $"Prenom: {catAbsence.Prenom}, " +
                                  $"DateAbsence: {catAbsence.DateAbsence}, " +
                                  $"AbsenceStatutId: {catAbsence.AbsenceStatutId}, " +
                                  $"Type: {catAbsence.Type}");
            }
        }
    }
}
