using System.Data;

namespace CronAbsence.Domain.Interfaces
{
    public interface IExcelReaderService
    {
        Task<DataTable> ReadDataAsync(string csvFilePath);
    }
}
