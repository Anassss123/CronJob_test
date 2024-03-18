using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

    namespace CronAbsence.Infrastructure.Service.Data
{
    public class DatabaseReaderService : IDatabaseReaderService
    {
        private readonly DataContext _context;

        public DatabaseReaderService(DataContext context)
        {
            _context = context;
        }

        public async Task<CatAbsenceTable> GetCatAbsencesTableAsync()
        {
            var absences = await _context.Cat_absence.ToListAsync();
            return new CatAbsenceTable { Absences = absences };
        }

        public async Task<List<CatAbsenceStatut>> GetCatAbsenceStatutsAsync()
        {
            return await _context.Cat_absence_statut.ToListAsync();
        }

        public class CatAbsenceTable
        {
            public List<CatAbsence> Absences { get; set; }
        }
    }
}

