using CronAbsence.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Data
{
    public interface IDatabaseReaderService
    {
        Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync();
    }
}
