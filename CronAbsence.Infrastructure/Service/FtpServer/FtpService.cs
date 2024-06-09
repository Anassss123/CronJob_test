using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CronAbsence.Infrastructure.Interfaces;

namespace CronAbsence.Api.Service
{
    public class FtpService : IFtpService
    {
        public async Task DownloadFileAsync(string ftpHost, string ftpPort, string ftpUserName, string ftpPassword, string fileName, string localFilePath)
        {
            string ftpFilePath = $"ftp://{ftpHost}:{ftpPort}/{fileName}";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (FileStream fileStream = File.Create(localFilePath))
            {
                responseStream.CopyTo(fileStream);
            }
        }
    }
}
