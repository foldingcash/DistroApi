namespace StatsDownload.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;

    public class StatsFileDateTimeFormatsAndOffsetProvider : IStatsFileDateTimeFormatsAndOffsetService
    {
        private readonly DateTimeFormatsSettings dateTimeFormatsSettings;

        public StatsFileDateTimeFormatsAndOffsetProvider(IOptions<DateTimeFormatsSettings> dateTimeFormatsSettings)
        {
            this.dateTimeFormatsSettings = dateTimeFormatsSettings?.Value
                                           ?? throw new ArgumentNullException(nameof(dateTimeFormatsSettings));
        }

        public (string format, int hourOffset)[] GetStatsFileDateTimeFormatsAndOffset()
        {
            (string format, int hourOffset)[] configuredFormatsAndOffset = GetConfiguredFormats();
            (string format, int hourOffset)[] codedFormatsAndOffset = GetCodedFormats();

            return codedFormatsAndOffset.Union(configuredFormatsAndOffset).ToArray();
        }

        private (string format, int hourOffset)[] GetCodedFormats()
        {
            return GetFormats(Constants.StatsFile.TimeZonesAndOffset);
        }

        private (string format, int hourOffset)[] GetConfiguredFormats()
        {
            return GetFormats(dateTimeFormatsSettings.Formats);
        }

        private (string format, int hourOffset)[] GetFormats(ICollection<DateTimeFormat> dateTimeFormats)
        {
            var formats = new List<(string, int)>();

            foreach (DateTimeFormat format in dateTimeFormats ?? Enumerable.Empty<DateTimeFormat>())
            {
                foreach (string template in Constants.StatsFile.DateTimeFormats)
                {
                    formats.Add((string.Format(template, format.Format), format.HourOffset));
                }
            }

            return formats.ToArray();
        }
    }
}