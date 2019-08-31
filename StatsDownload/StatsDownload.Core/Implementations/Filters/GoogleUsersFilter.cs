namespace StatsDownload.Core.Implementations.Filters
{
    using System;
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class GoogleUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly IGoogleUsersFilterSettings settings;

        public GoogleUsersFilter(IStatsFileParserService innerService, IGoogleUsersFilterSettings settings)
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
                    results.UsersData.Where(data =>
                        !data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ?? true),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}