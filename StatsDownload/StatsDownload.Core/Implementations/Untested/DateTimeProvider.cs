namespace StatsDownload.Core.Implementations.Untested
{
    using System;
    using Interfaces;

    public class DateTimeProvider : IDateTimeService
    {
        public DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
    }
}