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
            var newRecords = fileData.Where(f => !dbData.Any(d =>
                d.Matricule == f.Matricule &&
                d.DateAbsence == f.DateAbsence &&
                d.Type == f.Type)).ToList();
            
            // Set LastUpdate to NULL for new entries
            newRecords.ForEach(record => record.LastUpdate = null);

            await _databaseReaderService.InsertCatAbsencesAsync(newRecords);
        }

        public async Task UpdateExistingData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData)
        {
            var updatedRecords = fileData.Where(f => dbData.Any(d =>
                d.Matricule == f.Matricule &&
                d.DateAbsence == f.DateAbsence &&
                d.Type == f.Type &&
                d.AbsenceStatutId != f.AbsenceStatutId))
                .ToList();
            
            await _databaseReaderService.UpdateCatAbsencesAsync(updatedRecords);
        }

        public async Task DeleteOldData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData)
        {
            var deletedRecords = dbData.Where(d => !fileData.Any(f =>
                f.Matricule == d.Matricule &&
                f.DateAbsence == d.DateAbsence &&
                f.Type == d.Type))
                .ToList();
            
            await _databaseReaderService.DeleteCatAbsencesAsync(deletedRecords);
        }
    }
}
