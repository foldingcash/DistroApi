namespace StatsDownload.Core.Implementations.Filters
{
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class WhitespaceNameUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly IWhitespaceNameUsersFilterSettings settings;

        public WhitespaceNameUsersFilter(IStatsFileParserService innerService,
                                         IWhitespaceNameUsersFilterSettings settings)
        {
            this.innerService = innerService;
            this.settings = settings;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (settings.Enabled)
            {
                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data => !string.IsNullOrWhiteSpace(data.Name)), results.FailedUsersData);
            }

            return results;
        }
    }
}