namespace StatsDownload.Wrappers
{
    using System;

    using StatsDownload.Core.Interfaces;

    public class DateTimeProvider : IDateTimeService
    {
        public DateTime DateTimeNow()
        {
            return DateTime.UtcNow;
        }
    }
}