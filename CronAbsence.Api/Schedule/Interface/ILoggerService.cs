using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Api.Schedule.Interface
{
    public interface ILoggerService
    {
        void LogCatAbsences(IEnumerable<CatAbsence> catAbsences);
    }
}
