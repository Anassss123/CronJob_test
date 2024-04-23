using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Infrastructure.Service.Process
{
    public interface IDataComparer
    {
        Task<IEnumerable<CatAbsence>> CompareDataAsync(DataTable csvData);
    }
}
