using CronAbsence.Domain.Interfaces;
using CronAbsence.Domain.Models;

namespace CronAbsence.Infrastructure.Service.Process
{
    public class DataComparer : IDataComparer
    {
        private readonly IPilotageRepository _PilotageRepository;

        public DataComparer(IPilotageRepository PilotageRepository)
        {
            _PilotageRepository = PilotageRepository;
        }

        public async Task InsertNewAbsence(IEnumerable<CatAbsence> dbAbsence, IEnumerable<CatAbsence> fileAbsence)
        {
            var newRecords = fileAbsence.Where(fa => !dbAbsence.Any(da =>
                da.Matricule == fa.Matricule &&
                da.Date == fa.Date &&
                da.Motif == fa.Motif)).ToList();
            
            // Set LastUpdate to NULL for new entries
            newRecords.ForEach(record => record.Debut = null);
            newRecords.ForEach(record => record.Fin = null);

            await _PilotageRepository.InsertCatAbsencesAsync(newRecords);
        }

        public async Task UpdateExistingAbsence(IEnumerable<CatAbsence> dbAbsence, IEnumerable<CatAbsence> fileAbsence)
        {
            var updatedRecords = fileAbsence.Where(fa => dbAbsence.Any(da =>
                da.Matricule == fa.Matricule &&
                da.Date == fa.Date &&
                da.Motif == fa.Motif &&
                da.Type != fa.Type))
                .ToList();

            updatedRecords.ForEach(record => record.Debut = null);
            updatedRecords.ForEach(record => record.Fin = null);

            await _PilotageRepository.UpdateCatAbsencesAsync(updatedRecords);
        }

        public async Task CancelDeletedAbsence(IEnumerable<CatAbsence> dbAbsence, IEnumerable<CatAbsence> fileAbsence)
        {
            var deletedRecords = dbAbsence.Where(da => !fileAbsence.Any(fa =>
                fa.Matricule == da.Matricule &&
                fa.Date == da.Date &&
                fa.Motif == da.Motif))
                .ToList();

            // Set Type to 0 to indicate that's been deleted
            deletedRecords.ForEach(record => { 
                                        record.Type = 0;
                                        record.Flag = 0;
                                        });
                                        

            await _PilotageRepository.UpdateCatAbsencesAsync(deletedRecords);
        }
    }
}
