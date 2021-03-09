namespace StatsDownload.Core.TestHarness
{
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class TestHarnessOneHundredUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly TestHarnessSettings settings;

        public TestHarnessOneHundredUsersFilter(IStatsFileParserService innerService,
                                                IOptions<TestHarnessSettings> settings)
        {
            this.innerService = innerService;
            this.settings = settings.Value;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (settings.EnableOneHundredUsersFilter)
            {
                return new ParseResults(results.DownloadDateTime, results.UsersData.Take(100).ToArray(),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}