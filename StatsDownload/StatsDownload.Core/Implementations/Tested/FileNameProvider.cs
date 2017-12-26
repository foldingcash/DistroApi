namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FileNameProvider : IFileNameService
    {
        private const string FileName = "daily_user_summary.txt.bz2";

        private readonly IDateTimeService dateTimeService;

        public FileNameProvider(IDateTimeService dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public string GetNewFilePath(string directory)
        {
            DateTime dateTime = DateTimeNow();
            string fileName = $"{dateTime.ToFileTime()}.{FileName}";
            return Path.Combine(directory, fileName);
        }

        private DateTime DateTimeNow()
        {
            return dateTimeService.DateTimeNow();
        }
    }
}