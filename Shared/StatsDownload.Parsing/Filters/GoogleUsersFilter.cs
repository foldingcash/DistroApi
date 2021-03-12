namespace StatsDownload.Parsing.Filters
{
    using System;
    using System.Linq;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Settings;

    public class GoogleUsersFilter : IStatsFileParserService
    {
        private readonly FilterSettings filterSettings;

        private readonly IStatsFileParserService innerService;

        private readonly ILogger<GoogleUsersFilter> logger;

        public GoogleUsersFilter(ILogger<GoogleUsersFilter> logger, IStatsFileParserService innerService,
                                 IOptions<FilterSettings> filterSettings)
        {
            this.logger = logger;
            this.innerService = innerService;
            this.filterSettings = filterSettings.Value;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (filterSettings.EnableGoogleUsersFilter)
            {
                logger.LogDebug("Google users filtered enabled, filtering users out...");
                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data =>
                        !data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ?? true).ToArray(),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}