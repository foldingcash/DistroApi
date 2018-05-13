namespace StatsDownload.Core.Implementations.Untested
{
    using System;

    using StatsDownload.Core.Interfaces;

    public class DateTimeProvider : IDateTimeService
    {
        public DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
    }
}