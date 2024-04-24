using System;
using System.Collections.Generic;
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

        public async Task InsertNewData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData)
        {
            var newRecords = fileData.Where(f => !dbData.Any(d => d.Matricule == f.Matricule)).ToList();

            // Remove LastUpdate for new entries
            foreach (var newRecord in newRecords)
            {
                newRecord.LastUpdate = null;
            }

            await _databaseReaderService.InsertCatAbsencesAsync(newRecords);
        }

        public async Task UpdateExistingData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData)
        {
            var updatedRecords = fileData.Where(f => dbData.Any(d => d.Matricule == f.Matricule &&
                                                                (d.Nom != f.Nom ||
                                                                 d.Prenom != f.Prenom ||
                                                                 d.DateAbsence != f.DateAbsence ||
                                                                 d.AbsenceStatutId != f.AbsenceStatutId ||
                                                                 d.Type != f.Type)))
                                         .ToList();

            // Set LastUpdate only when AbsenceStatutId changes
            foreach (var updatedRecord in updatedRecords)
            {
                var matchingDbRecord = dbData.FirstOrDefault(d => d.Matricule == updatedRecord.Matricule);
                if (matchingDbRecord != null && matchingDbRecord.AbsenceStatutId != updatedRecord.AbsenceStatutId)
                {
                    updatedRecord.LastUpdate = DateTime.Now;
                }
                else
                {
                    updatedRecord.LastUpdate = null; // No change, so remove LastUpdate
                }
            }

            await _databaseReaderService.UpdateCatAbsencesAsync(updatedRecords);
        }

        public async Task DeleteOldData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData)
        {
            var deletedRecords = dbData.Where(d => !fileData.Any(f => f.Matricule == d.Matricule)).ToList();
            await _databaseReaderService.DeleteCatAbsencesAsync(deletedRecords);
        }
    }
}
