using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Interfaces
{
    public interface IFTPProvider
    {
        Task DownloadFileAsync(string fileName, string localFilePath);
        string GetLocalDownloadPath(string fileName);
        string GetLocalArchivePath(string fileName);
        string GetLocalOriginalPath(string fileName);
    }
}
