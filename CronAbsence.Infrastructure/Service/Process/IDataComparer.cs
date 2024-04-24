using System.Collections.Generic;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Infrastructure.Service.Process
{
    public interface IDataComparer
    {
        Task InsertNewData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData);

        Task UpdateExistingData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData);

        Task DeleteOldData(IEnumerable<CatAbsence> dbData, IEnumerable<CatAbsence> fileData);
    }
}
