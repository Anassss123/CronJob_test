using System;
using System.Collections.Generic;
using System.Data;
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

                // Convert CSV DataTable to CatAbsence list
                var csvAbsences = ConvertDataTableToCatAbsenceList(csvData);

                // Log the CSV data for verification
                Console.WriteLine("CSV Data:");
                LogDataTable(csvData);

                // Get data from the database
                var dbData = await _databaseReaderService.GetCatAbsencesAsync();

                // Compare data and perform operations
                await _dataComparer.InsertNewData(dbData, csvAbsences);
                await _dataComparer.UpdateExistingData(dbData, csvAbsences);
                await _dataComparer.DeleteOldData(dbData, csvAbsences);

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

        private List<CatAbsence> ConvertDataTableToCatAbsenceList(DataTable dataTable)
        {
            var catAbsences = new List<CatAbsence>();
            foreach (DataRow row in dataTable.Rows)
            {
                var catAbsence = new CatAbsence
                {
                    Matricule = Convert.ToInt32(row["Matricule"]),
                    Nom = row["Nom"].ToString(),
                    Prenom = row["Prenom"].ToString(),
                    DateAbsence = Convert.ToDateTime(row["DateAbsence"]),
                    AbsenceStatutId = Convert.ToInt32(row["AbsenceStatutId"]),
                    Type = row["Type"].ToString()
                };
                catAbsences.Add(catAbsence);
            }
            return catAbsences;
        }
    }
}
