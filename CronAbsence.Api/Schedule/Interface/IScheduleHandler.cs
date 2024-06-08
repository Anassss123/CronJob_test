using System.Threading.Tasks;

namespace CronAbsence.Api.Service
{
    public interface IScheduleHandler
    {
        Task ProcessAsync();
    }
}
