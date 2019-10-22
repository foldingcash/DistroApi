namespace StatsDownload.Core.Interfaces.Exceptions
{
    using System;

    public class FileDownloadFailedCompressionException : Exception
    {
        public FileDownloadFailedCompressionException()
        {
        }

        public FileDownloadFailedCompressionException(string message)
            : base(message)
        {
        }

        public FileDownloadFailedCompressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}