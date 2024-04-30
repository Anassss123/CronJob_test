using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Infrastructure.Service.Process
{
    public interface IDataComparer
    {
        Task InsertNewAbsence(IEnumerable<CatAbsence> dbAbsence, IEnumerable<CatAbsence> fileAbsence);

        Task UpdateExistingAbsence(IEnumerable<CatAbsence> dbAbsence, IEnumerable<CatAbsence> fileAbsence);

        Task CancelDeletedAbsence(IEnumerable<CatAbsence> dbAbsence, IEnumerable<CatAbsence> fileAbsence);
    }
}
