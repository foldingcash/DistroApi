namespace StatsDownload.Core.Implementations
{
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;

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

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);

            if (settings.Enabled)
            {
                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data => !string.IsNullOrWhiteSpace(data.Name)),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}