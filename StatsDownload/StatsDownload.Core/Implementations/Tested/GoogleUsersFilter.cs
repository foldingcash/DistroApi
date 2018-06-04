using System;
using System.Linq;
using StatsDownload.Core.Interfaces;
using StatsDownload.Core.Interfaces.DataTransfer;

namespace StatsDownload.Core.Implementations.Tested
{
    public class GoogleUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly IGoogleUsersFilterSettings settings;

        public GoogleUsersFilter(IStatsFileParserService innerService, IGoogleUsersFilterSettings settings)
        {
            this.innerService = innerService;
            this.settings = settings;
        }

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);

            if (settings.Enabled)
            {
                return
                    new ParseResults(
                        results.UsersData.Where(
                            data => !data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ??
                                    true),
                        results.FailedUsersData);
            }

            return results;
        }
    }
}