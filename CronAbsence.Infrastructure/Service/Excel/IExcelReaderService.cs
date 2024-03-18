using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Excel
{
    public interface IExcelReaderService
    {
        DataTable ReadData(FileInfo file);
        Task<object[,]> ReadDataAsync(FileInfo file);
        void DisplayDataAsync(DataTable dataTable);
    }
}
