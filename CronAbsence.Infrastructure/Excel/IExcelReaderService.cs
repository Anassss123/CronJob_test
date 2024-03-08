using System.IO;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Excel
{
    public interface IExcelReaderService
    {
        Task<object[,]> ReadDataAsync(FileInfo file);
    }
}
