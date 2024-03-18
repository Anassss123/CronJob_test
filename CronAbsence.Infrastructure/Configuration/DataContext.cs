using CronAbsence.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CronAbsence.Infrastructure.Configuration
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Parameterless constructor for design-time operations
        public DbSet<CatAbsence> Cat_absence { get; set; }
        public DbSet<CatAbsenceStatut> Cat_absence_statut { get; set; }

    }
}
