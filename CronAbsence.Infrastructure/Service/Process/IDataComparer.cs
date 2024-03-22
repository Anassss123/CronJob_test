using CronAbsence.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Process
{
    public interface IDataComparer
    {
        Task<IEnumerable<CatAbsence>> CompareDataAsync(DataTable excelData);
    }
}
