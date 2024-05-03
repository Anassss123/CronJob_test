using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Infrastructure.Service.Process;
using Dapper;

namespace CronAbsence.Api.Service
{
    public class ScheduleHandler : IScheduleHandler
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IDataComparer _dataComparer;
        private readonly IDatabaseReaderService _databaseReaderService;
        private readonly string _connectionString;

        public ScheduleHandler(IExcelReaderService excelReaderService, IDataComparer dataComparer, IDatabaseReaderService databaseReaderService ,IConfiguration configuration )
        {
            _excelReaderService = excelReaderService;
            _dataComparer = dataComparer;
            _databaseReaderService = databaseReaderService;
            _connectionString = configuration.GetConnectionString("Default");
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
                var fileAbsences = ConvertDataTableToCatAbsenceList(csvData);

                // Log the CSV data for verification
                Console.WriteLine("CSV Data:");
                LogCatAbsences(fileAbsences);

                // Get data from the database
                var dbAbsence = await _databaseReaderService.GetCatAbsencesAsync();

                // Compare data and perform operations
                await _dataComparer.InsertNewAbsence(dbAbsence, fileAbsences);
                await _dataComparer.UpdateExistingAbsence(dbAbsence, fileAbsences);
                await _dataComparer.CancelDeletedAbsence(dbAbsence, fileAbsences);

                // archivage

                // PPM Api

                Console.WriteLine("Processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void LogCatAbsences(IEnumerable<CatAbsence> catAbsences)
        {
            foreach (var absence in catAbsences)
            {
                Console.WriteLine($"Matricule: {absence.Matricule} | Nom: {absence.Nom} | Prenom: {absence.Prenom} | Date: {absence.Date} | Type: {absence.Type} | Debut: {absence.Debut} | Fin: {absence.Fin} | Motif: {absence.Motif} | Flag: {absence.Flag}");
            }
        }

        private List<CatAbsence> ConvertDataTableToCatAbsenceList(DataTable dataTable)
        {
            var catAbsences = new List<CatAbsence>();
            int maxId = GetMaxIdFromDatabase(); // Start ID counter from 1

            foreach (DataRow row in dataTable.Rows)
            {
                maxId++;
                var catAbsence = new CatAbsence
                {
                    ID = maxId,
                    Matricule = Convert.ToInt32(row["Matricule"]),
                    Nom = row["Nom"].ToString(),
                    Prenom = row["Prenom"].ToString(),
                    Date = DateTime.ParseExact(row["Date"].ToString(), "dd/MM/yyyy", null),
                    Type = ParseType(row["Nombre Heures"].ToString()), 
                    Debut = row["Debut"].ToString(),
                    Fin = row["Fin"].ToString(),
                    Motif = row["Motif"].ToString().Equals("REPOS ASTREINTE", StringComparison.InvariantCultureIgnoreCase) ? "REPOS ASTREINTE" : "ABSENCE",
                    
                };

                catAbsences.Add(catAbsence);
            }
            return catAbsences;
        }


        private int ParseType(string nombreHeures)
        {
            // Logic to parse "Nombre Heures" to Type
            
            if (nombreHeures.StartsWith("J"))
                return 3;
            else if (nombreHeures.StartsWith("M"))
                return 1;
            else if (nombreHeures.StartsWith("A"))
                return 2;
             else
                throw new ArgumentException("Invalid value for Nombre Heures");// Other values 
        }

        private int GetMaxIdFromDatabase()
        {
            using (var connection = new SqlConnection(_connectionString)) // Use your connection string
            {
                connection.Open();
                string query = "SELECT MAX(ID) FROM Cat_absence";
                int maxId = connection.ExecuteScalar<int>(query);
                return maxId;
            }
        }

    }
}
