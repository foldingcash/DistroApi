namespace StatsDownload.Core.Implementations.Tested
{
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class ZeroPointUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly IZeroPointUsersFilterSettings settings;

        public ZeroPointUsersFilter(IStatsFileParserService innerService, IZeroPointUsersFilterSettings settings)
        {
            this.innerService = innerService;
            this.settings = settings;
        }

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);

            if (settings.Enabled)
            {
                return new ParseResults(results.UsersData.Where(data => data.TotalPoints > 0), results.FailedUsersData);
            }

            return results;
        }
    }
}