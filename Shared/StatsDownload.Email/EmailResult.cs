namespace StatsDownload.Email
{
    using System;

    public class EmailResult
    {
        public EmailResult()
        {
            Success = true;
        }

        public EmailResult(Exception error)
        {
            Success = false;
            Error = error;
        }

        public Exception Error { get; }

        public bool Success { get; }
    }
}