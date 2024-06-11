using CronAbsence.Api.Schedule.Interface;
using CronAbsence.Domain.Models;

namespace CronAbsence.Api.Service
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogCatAbsences(IEnumerable<CatAbsence> catAbsences)
        {
            foreach (var absence in catAbsences)
            {
                _logger.LogInformation("Matricule: {Matricule} | Nom: {Nom} | Prenom: {Prenom} | Date: {Date} | Type: {Type} | Debut: {Debut} | Fin: {Fin} | Motif: {Motif} | Flag: {Flag}",
                    absence.Matricule, absence.Nom, absence.Prenom, absence.Date, absence.Type, absence.Debut, absence.Fin, absence.Motif, absence.Flag);
            }
        }
    }
}
