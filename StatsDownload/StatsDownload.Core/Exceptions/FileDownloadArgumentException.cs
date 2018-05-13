namespace StatsDownload.Core.Exceptions
{
    using System;

    public class FileDownloadArgumentException : Exception
    {
        public FileDownloadArgumentException(string message)
            : base(message)
        {
        }
    }
}