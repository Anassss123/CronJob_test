using CronAbsence.Domain.Models;

namespace CronAbsence.Domain.Interfaces
{
    public interface IPilotageRepository
    {
        Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync();

        Task InsertCatAbsencesAsync(IEnumerable<CatAbsence> newAbsences);

        Task UpdateCatAbsencesAsync(IEnumerable<CatAbsence> updatedAbsences);
        int GetMaxIdFromDatabase();

    }
}
