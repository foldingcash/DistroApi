namespace StatsDownload.Core
{
    using System;

    public class DateTimeProvider : IDateTimeService
    {
        public DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
    }
}