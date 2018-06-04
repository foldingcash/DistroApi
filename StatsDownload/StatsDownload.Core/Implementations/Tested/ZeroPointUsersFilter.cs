using System.Linq;
using StatsDownload.Core.Interfaces;
using StatsDownload.Core.Interfaces.DataTransfer;

namespace StatsDownload.Core.Implementations.Tested
{
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
            if (settings.Enabled)
            {
                ParseResults results = innerService.Parse(fileData);
                return new ParseResults(results.UsersData.Where(data => data.TotalPoints > 0), results.FailedUsersData);
            }

            return innerService.Parse(fileData);
        }
    }
}