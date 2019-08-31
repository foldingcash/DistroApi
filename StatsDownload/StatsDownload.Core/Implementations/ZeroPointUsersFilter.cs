namespace StatsDownload.Core.Implementations
{
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class ZeroPointUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly IZeroPointUsersFilterSettings settings;

        public ZeroPointUsersFilter(IStatsFileParserService innerService, IZeroPointUsersFilterSettings settings)
        {
            this.innerService = innerService;
            this.settings = settings;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (settings.Enabled)
            {
                return new ParseResults(results.DownloadDateTime, results.UsersData.Where(data => data.TotalPoints > 0),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}