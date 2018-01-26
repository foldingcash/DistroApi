namespace StatsDownload.Email
{
    using System;

    public class EmailArgumentException : ArgumentException
    {
        public EmailArgumentException(string parameterName, string message)
            : base($"Parameter '{parameterName}' is an invalid argument. Message: {message}")
        {
        }
    }
}