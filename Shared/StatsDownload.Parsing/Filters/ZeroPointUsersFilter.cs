namespace StatsDownload.Parsing.Filters
{
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class ZeroPointUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly FilterSettings settings;

        public ZeroPointUsersFilter(IStatsFileParserService innerService, IOptions<FilterSettings> settings)
        {
            this.innerService = innerService;
            this.settings = settings.Value;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (settings.EnableZeroPointUsersFilter)
            {
                return new ParseResults(results.DownloadDateTime, results.UsersData.Where(data => data.TotalPoints > 0),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}