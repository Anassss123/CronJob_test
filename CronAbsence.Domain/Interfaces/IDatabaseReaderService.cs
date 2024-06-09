using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Domain.Interfaces
{
    public interface IDatabaseReaderService
    {
        Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync();

        Task InsertCatAbsencesAsync(IEnumerable<CatAbsence> newAbsences);

        Task UpdateCatAbsencesAsync(IEnumerable<CatAbsence> updatedAbsences);
        int GetMaxIdFromDatabase();

    }
}
