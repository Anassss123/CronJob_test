using System.Net;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace CronAbsence.Api.Service
{
    public class FTPProvider : IFTPProvider
    {
        private readonly FtpOptions _ftpOptions;

        public FTPProvider(IOptions<FtpOptions> ftpOptions)
        {
            _ftpOptions = ftpOptions.Value;
        }

        public async Task DownloadFileAsync(string fileName, string localFilePath)
        {
            string ftpFilePath = $"ftp://{_ftpOptions.Host}:{_ftpOptions.Port}/{fileName}";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_ftpOptions.User, _ftpOptions.Password);

            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (FileStream fileStream = File.Create(localFilePath))
            {
                responseStream.CopyTo(fileStream);
            }
        }

        public string GetLocalDownloadPath(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        public string GetLocalArchivePath(string fileName)
        {
            return Path.Combine(_ftpOptions.DestinationFolderPath, fileName);
        }

        public string GetLocalOriginalPath(string fileName)
        {
            return Path.Combine(_ftpOptions.SourceFolderPath, fileName);
        }
    }
}
