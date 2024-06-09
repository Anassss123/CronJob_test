using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Interfaces
{
    public interface IFtpService
    {
        Task DownloadFileAsync(string ftpHost, string ftpPort, string ftpUserName, string ftpPassword, string fileName, string localFilePath);
    }
}