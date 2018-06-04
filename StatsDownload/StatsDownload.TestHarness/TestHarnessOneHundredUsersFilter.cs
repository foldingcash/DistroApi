namespace StatsDownload.TestHarness
{
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class TestHarnessOneHundredUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        public TestHarnessOneHundredUsersFilter(IStatsFileParserService innerService)
        {
            this.innerService = innerService;
        }

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);
            return new ParseResults(results.UsersData.Take(100), results.FailedUsersData);
        }
    }
}