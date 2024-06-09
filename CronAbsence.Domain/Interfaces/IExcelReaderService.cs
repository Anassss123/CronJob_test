using System.Data;
using System.Threading.Tasks;

namespace CronAbsence.Domain.Interfaces
{
    public interface IExcelReaderService
    {
        Task<DataTable> ReadDataAsync(string csvFilePath);
    }
}
