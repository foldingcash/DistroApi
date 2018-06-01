namespace StatsDownload.Core.Implementations.Tested
{
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class NoPaymentAddressUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        public NoPaymentAddressUsersFilter(IStatsFileParserService innerService)
        {
            this.innerService = innerService;
        }

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);
            return new ParseResults(results.UsersData.Where(data => !string.IsNullOrWhiteSpace(data.BitcoinAddress)),
                results.FailedUsersData);
        }
    }
}