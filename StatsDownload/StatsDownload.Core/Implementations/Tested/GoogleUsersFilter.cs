namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class GoogleUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        public GoogleUsersFilter(IStatsFileParserService innerService)
        {
            this.innerService = innerService;
        }

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);
            return
                new ParseResults(
                    results.UsersData.Where(
                        data => !data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ?? true),
                    results.FailedUsersData);
        }
    }
}