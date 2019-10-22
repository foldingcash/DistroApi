namespace StatsDownload.Core.Interfaces.DataTransfer
{
    using System;

    public class ValidatedFile
    {
        public ValidatedFile()
        {

        }

        public ValidatedFile(int downloadId, DateTime downloadDateTime, string filePath)
        {
            DownloadId = downloadId;
            DownloadDateTime = downloadDateTime;
            FilePath = filePath;
        }

        public DateTime DownloadDateTime { get; }

        public int DownloadId { get; }

        public string FilePath { get; }
    }
}