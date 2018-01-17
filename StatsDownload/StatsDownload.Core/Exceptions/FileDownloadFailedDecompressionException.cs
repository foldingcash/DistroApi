namespace StatsDownload.Core
{
    using System;

    public class FileDownloadFailedDecompressionException : Exception
    {
        public FileDownloadFailedDecompressionException()
        {
        }

        public FileDownloadFailedDecompressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}