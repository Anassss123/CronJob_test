using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;
using static CronAbsence.Infrastructure.Service.Data.DatabaseReaderService;

namespace CronAbsence.Infrastructure.Service.Data
{
    public interface IDatabaseReaderService
    {
        Task<CatAbsenceTable> GetCatAbsencesTableAsync();
        Task<List<CatAbsenceStatut>> GetCatAbsenceStatutsAsync();
    }
}
