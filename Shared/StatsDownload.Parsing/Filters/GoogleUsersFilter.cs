namespace StatsDownload.Parsing.Filters
{
    using System;
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class GoogleUsersFilter : IStatsFileParserService
    {
        private readonly FilterSettings filterSettings;

        private readonly IStatsFileParserService innerService;

        public GoogleUsersFilter(IStatsFileParserService innerService, IOptions<FilterSettings> filterSettings)
        {
            this.innerService = innerService;
            this.filterSettings = filterSettings.Value;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (filterSettings.EnableGoogleUsersFilter)
            {
                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data =>
                        !data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ?? true),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}