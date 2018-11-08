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
            (string format, int hourOffset)[] configuredFormatsAndOffset = GetConfiguredFormatsAndOffset();

            return Constants.StatsFile.DateTimeFormatsAndOffset.Union(configuredFormatsAndOffset).ToArray();
        }

        private (string format, int hourOffset)[] GetConfiguredFormatsAndOffset()
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