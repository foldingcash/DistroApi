namespace StatsDownload.Core.Implementations.Tested
{
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class NoPaymentAddressUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly INoPaymentAddressUsersFilterSettings settings;

        public NoPaymentAddressUsersFilter(IStatsFileParserService innerService,
            INoPaymentAddressUsersFilterSettings settings)
        {
            this.innerService = innerService;
            this.settings = settings;
        }

        public ParseResults Parse(string fileData)
        {
            ParseResults results = innerService.Parse(fileData);

            if (settings.Enabled)
            {
                return new ParseResults(
                    results.UsersData.Where(data => !string.IsNullOrWhiteSpace(data.BitcoinAddress)),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}