namespace CronAbsence.Domain.Models
{
    public class FtpOptions
    {
        public string Host {get; set;}
        public string Port {get; set;}
        public string User {get; set;}
        public string Password {get; set;}
        public string DestinationFolderPath {get; set;}
        public string SourceFolderPath {get; set;}
    }
}