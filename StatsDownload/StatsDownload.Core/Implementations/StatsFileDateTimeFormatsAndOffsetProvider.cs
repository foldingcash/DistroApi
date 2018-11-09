namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;

    public class StatsFileDateTimeFormatsAndOffsetProvider : IStatsFileDateTimeFormatsAndOffsetService
    {
        private readonly IStatsFileDateTimeFormatsAndOffsetSettings statsFileDateTimeFormatsAndOffsetSettings;

        public StatsFileDateTimeFormatsAndOffsetProvider(
            IStatsFileDateTimeFormatsAndOffsetSettings statsFileDateTimeFormatsAndOffsetSettings)
        {
            this.statsFileDateTimeFormatsAndOffsetSettings = statsFileDateTimeFormatsAndOffsetSettings ??
                                                             throw new ArgumentNullException(
                                                                 nameof(statsFileDateTimeFormatsAndOffsetSettings));
        }

        public (string format, int hourOffset)[] GetStatsFileDateTimeFormatsAndOffset()
        {
            (string format, int hourOffset)[] configuredFormatsAndOffset = GetConfiguredFormats();
            (string format, int hourOffset)[] codedFormatsAndOffset = GetCodedFormats();

            return codedFormatsAndOffset.Union(configuredFormatsAndOffset).ToArray();
        }

        private (string format, int hourOffset)[] GetCodedFormats()
        {
            var codedFormatsAndOffset = new List<(string format, int hourOffset)>();

            foreach ((string timeZone, int hourOffset) timeZoneAndOffset in Constants.StatsFile.TimeZonesAndOffset)
            {
                foreach (string dateTimeFormat in Constants.StatsFile.DateTimeFormats)
                {
                    codedFormatsAndOffset.Add((string.Format(dateTimeFormat, timeZoneAndOffset.timeZone),
                        timeZoneAndOffset.hourOffset));
                }
            }

            return codedFormatsAndOffset.ToArray();
        }

        private (string format, int hourOffset)[] GetConfiguredFormats()
        {
            string settings = statsFileDateTimeFormatsAndOffsetSettings.GetStatsFileTimeZoneAndOffsetSettings();

            string[] settingsSplit = settings?.Split(';', '=');

            var configuredFormatsAndOffset = new List<(string format, int hourOffset)>();

            for (var index = 0; index < settingsSplit?.Length && index + 1 < settingsSplit.Length; index++)
            {
                string timeZone = settingsSplit[index];
                index++;
                string offset = settingsSplit[index];

                foreach (string dateTimeFormat in Constants.StatsFile.DateTimeFormats)
                {
                    configuredFormatsAndOffset.Add((string.Format(dateTimeFormat, timeZone), int.Parse(offset)));
                }
            }

            return configuredFormatsAndOffset.ToArray();
        }
    }
}