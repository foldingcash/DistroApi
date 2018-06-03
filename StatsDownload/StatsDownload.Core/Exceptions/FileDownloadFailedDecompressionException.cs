namespace StatsDownload.Core.Exceptions
{
    using System;

    public class FileDownloadFailedDecompressionException : Exception
    {
        public FileDownloadFailedDecompressionException(string message)
            : base(message)
        {
        }

        public FileDownloadFailedDecompressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}