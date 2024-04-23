using System.Data;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Excel
{
    public interface IExcelReaderService
    {
        Task<DataTable> ReadDataAsync(string csvFilePath);
    }
}
