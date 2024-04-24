using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Infrastructure.Service.Data
{
    public interface IDatabaseReaderService
    {
        Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync();

        Task InsertCatAbsencesAsync(IEnumerable<CatAbsence> newAbsences);

        Task UpdateCatAbsencesAsync(IEnumerable<CatAbsence> updatedAbsences);

        Task DeleteCatAbsencesAsync(IEnumerable<CatAbsence> deletedAbsences);
    }
}
