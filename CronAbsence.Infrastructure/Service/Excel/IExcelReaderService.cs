using System.Data;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Excel
{
    public interface IExcelReaderService
    {
        Task<DataTable> ReadDataAsync(string ftpServer, string ftpUsername, string ftpPassword, string remoteFilePath, string localFilePath);
    }
}
