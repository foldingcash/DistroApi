namespace StatsDownload.Core
{
    using System;

    public class FileDownloadArgumentException : Exception
    {
        public FileDownloadArgumentException()
        {
        }

        public FileDownloadArgumentException(string message)
            : base(message)
        {
        }
    }
}