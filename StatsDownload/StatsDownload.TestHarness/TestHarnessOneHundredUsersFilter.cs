namespace StatsDownload.TestHarness
{
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class TestHarnessOneHundredUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly ITestHarnessSettingsService settingsService;

        public TestHarnessOneHundredUsersFilter(IStatsFileParserService innerService,
                                                ITestHarnessSettingsService settingsService)
        {
            this.innerService = innerService;
            this.settingsService = settingsService;
        }

        public ParseResults Parse(string fileData)
        {
            if (settingsService.IsOneHundredUsersFilterEnabled())
            {
                ParseResults results = innerService.Parse(fileData);
                return new ParseResults(results.UsersData.Take(100), results.FailedUsersData);
            }

            return innerService.Parse(fileData);
        }
    }
}