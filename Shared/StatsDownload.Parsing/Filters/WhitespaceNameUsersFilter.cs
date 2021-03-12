namespace StatsDownload.Parsing.Filters
{
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Settings;

    public class WhitespaceNameUsersFilter : IStatsFileParserService
    {
        private readonly FilterSettings filterSettings;

        private readonly IStatsFileParserService innerService;

        public WhitespaceNameUsersFilter(IStatsFileParserService innerService, IOptions<FilterSettings> filterSettings)
        {
            this.innerService = innerService;
            this.filterSettings = filterSettings.Value;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (filterSettings.EnableWhitespaceNameUsersFilter)
            {
                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data => !string.IsNullOrWhiteSpace(data.Name)).ToArray(),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}