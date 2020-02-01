namespace StatsDownloadApi.Interfaces
{
    using System;

    public class InvalidDistributionState : Exception
    {
        public InvalidDistributionState(string message)
            : base(message)
        {
        }
    }
}