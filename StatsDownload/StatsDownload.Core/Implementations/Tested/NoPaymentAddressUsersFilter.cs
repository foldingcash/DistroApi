using System.Linq;
using StatsDownload.Core.Interfaces;
using StatsDownload.Core.Interfaces.DataTransfer;

namespace StatsDownload.Core.Implementations.Tested
{
    public class NoPaymentAddressUsersFilter : IStatsFileParserService
    {
        private readonly IStatsFileParserService innerService;

        private readonly INoPaymentAddressUsersSettings settings;

        public NoPaymentAddressUsersFilter(IStatsFileParserService innerService,
            INoPaymentAddressUsersSettings settings)
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