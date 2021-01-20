namespace StatsDownload.Core.Interfaces.Settings
{
    using System.Collections.Generic;

    public class DateTimeSettings
    {
        public ICollection<DateTimeFormat> Formats { get; set; }
    }
}