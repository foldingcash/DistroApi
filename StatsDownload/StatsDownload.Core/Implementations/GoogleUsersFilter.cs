namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;

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
                    new ParseResults(results.DownloadDateTime,
                        results.UsersData.Where(
                            data => !data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ??
                                    true),
                        results.FailedUsersData);
            }

            return results;
        }
    }
}