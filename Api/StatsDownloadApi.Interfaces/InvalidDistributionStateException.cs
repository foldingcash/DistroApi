namespace StatsDownloadApi.Interfaces
{
    using System;

    public class InvalidDistributionStateException : Exception
    {
        public InvalidDistributionStateException(string message)
            : base(message)
        {
        }
    }
}