using CronAbsence.Domain.Models;

namespace CronAbsence.Api.Schedule.Interface
{
    public interface ILoggerService
    {
        void LogCatAbsences(IEnumerable<CatAbsence> catAbsences);
    }
}
