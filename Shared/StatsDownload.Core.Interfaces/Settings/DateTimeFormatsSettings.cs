namespace StatsDownload.Core.Interfaces.Settings
{
    using System.Collections.Generic;

    public class DateTimeFormatsSettings
    {
        public ICollection<DateTimeFormat> Formats { get; set; }
    }
}