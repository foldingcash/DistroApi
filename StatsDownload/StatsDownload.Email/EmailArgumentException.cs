namespace StatsDownload.Email
{
    using System;

    public class EmailArgumentException : ArgumentException
    {
        public EmailArgumentException(string message)
            : base(message)
        {
        }
    }
}