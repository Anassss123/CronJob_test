using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Infrastructure.Service.Data
{
    public interface IDatabaseReaderService
    {
        Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync();
        Task UpdateCatAbsencesAsync(IEnumerable<CatAbsence> catAbsences);
    }
}
