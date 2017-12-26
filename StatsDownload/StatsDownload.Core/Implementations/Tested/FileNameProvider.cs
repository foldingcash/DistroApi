namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FileNameProvider : IFileNameService
    {
        private const string FileName = "daily_user_summary.txt.bz2";

        private const string UncompressedFileName = "daily_user_summary.txt";

        private readonly IDateTimeService dateTimeService;

        public FileNameProvider(IDateTimeService dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public string GetFileDownloadPath(string directory)
        {
            return GetFileDownloadPath(directory, FileName);
        }

        public string GetUncompressedFileDownloadPath(string directory)
        {
            return GetFileDownloadPath(directory, UncompressedFileName);
        }

        private DateTime DateTimeNow()
        {
            return dateTimeService.DateTimeNow();
        }

        private string GetFileDownloadPath(string directory, string fileName)
        {
            DateTime dateTime = DateTimeNow();
            string fileNameWithTime = $"{dateTime.ToFileTime()}.{fileName}";
            return Path.Combine(directory, fileNameWithTime);
        }
    }
}