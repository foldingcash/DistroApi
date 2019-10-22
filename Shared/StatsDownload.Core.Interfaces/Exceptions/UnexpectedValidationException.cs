namespace StatsDownload.Core.Interfaces.Exceptions
{
    using System;

    public class UnexpectedValidationException : Exception
    {
        public UnexpectedValidationException()
        {
        }

        public UnexpectedValidationException(string message)
            : base(message)
        {
        }

        public UnexpectedValidationException(Exception innerException)
            : base(string.Empty, innerException)
        {
        }

        public UnexpectedValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}