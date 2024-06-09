using System;
using System.Collections.Generic;
using CronAbsence.Api.Schedule.Interface;
using CronAbsence.Domain.Models;

namespace CronAbsence.Api.Service
{
    public class LoggerService : ILoggerService
    {
        public void LogCatAbsences(IEnumerable<CatAbsence> catAbsences)
        {
            foreach (var absence in catAbsences)
            {
                Console.WriteLine($"Matricule: {absence.Matricule} | Nom: {absence.Nom} | Prenom: {absence.Prenom} | Date: {absence.Date} | Type: {absence.Type} | Debut: {absence.Debut} | Fin: {absence.Fin} | Motif: {absence.Motif} | Flag: {absence.Flag}");
            }
        }
    }
}
