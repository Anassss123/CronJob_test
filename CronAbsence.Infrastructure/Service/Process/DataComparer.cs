using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;

namespace CronAbsence.Infrastructure.Service.Process
{
    public class DataComparer : IDataComparer
    {
        private readonly IDatabaseReaderService _databaseReaderService;

        public DataComparer(IDatabaseReaderService databaseReaderService)
        {
            _databaseReaderService = databaseReaderService;
        }

        public async Task<IEnumerable<CatAbsence>> CompareDataAsync(DataTable csvData)
        {
            // Get existing CatAbsence records from the database
            var dbCatAbsences = await _databaseReaderService.GetCatAbsencesAsync();

            // List to hold updated or new CatAbsence objects
            var updatedDataList = new List<CatAbsence>();

            // Iterate through each row in the CSV data
            foreach (DataRow csvRow in csvData.Rows)
            {
                // Extract Matricule from the CSV row
                var matricule = Convert.ToInt32(csvRow["Matricule"]);

                // Check if a CatAbsence with the same Matricule exists in the database
                var dbAbsence = dbCatAbsences.FirstOrDefault(a => a.Matricule == matricule);

                if (dbAbsence != null)
                {
                    // If CatAbsence exists, check if AbsenceStatutId has changed
                    var newAbsenceStatutId = Convert.ToInt32(csvRow["AbsenceStatutId"]);
                    if (dbAbsence.AbsenceStatutId != newAbsenceStatutId)
                    {
                        // If AbsenceStatutId has changed, update LastUpdated and AbsenceStatutId
                        dbAbsence.AbsenceStatutId = newAbsenceStatutId;
                        dbAbsence.LastUpdate = DateTime.Now;
                        updatedDataList.Add(dbAbsence);
                    }
                }
                else
                {
                    // If CatAbsence doesn't exist, create a new one with CSV data
                    var newAbsence = new CatAbsence
                    {
                        Matricule = matricule,
                        Nom = Convert.ToString(csvRow["Nom"]),
                        Prenom = Convert.ToString(csvRow["Prenom"]),
                        DateAbsence = Convert.ToDateTime(csvRow["DateAbsence"]),
                        AbsenceStatutId = Convert.ToInt32(csvRow["AbsenceStatutId"]),
                        Type = Convert.ToString(csvRow["Type"])
                    };

                    // Add the new CatAbsence to the list
                    updatedDataList.Add(newAbsence);
                }
            }

            // Return the list of updated or new CatAbsence objects
            return updatedDataList;
        }
    }
}
