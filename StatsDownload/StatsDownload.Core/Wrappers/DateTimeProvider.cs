namespace StatsDownload.Core.Wrappers
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