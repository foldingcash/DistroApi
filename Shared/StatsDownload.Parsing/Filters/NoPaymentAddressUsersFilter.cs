namespace StatsDownload.Parsing.Filters
{
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Settings;

    public class NoPaymentAddressUsersFilter : IStatsFileParserService
    {
        private readonly FilterSettings filterSettings;

        private readonly IStatsFileParserService innerService;

        public NoPaymentAddressUsersFilter(IStatsFileParserService innerService,
                                           IOptions<FilterSettings> filterSettings)
        {
            this.innerService = innerService;
            this.filterSettings = filterSettings.Value;
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            ParseResults results = innerService.Parse(filePayload);

            if (filterSettings.EnableNoPaymentAddressUsersFilter)
            {
                bool HasBitcoinAddress(UserData user) => !string.IsNullOrWhiteSpace(user.BitcoinAddress);
                bool HasBitcoinCashAddress(UserData user) => !string.IsNullOrWhiteSpace(user.BitcoinCashAddress);
                bool HasSlpAddress(UserData user) => !string.IsNullOrWhiteSpace(user.SlpAddress);
                bool HasCashTokensAddress(UserData user) => !string.IsNullOrWhiteSpace(user.CashTokensAddress);

                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data => HasBitcoinAddress(data) || HasBitcoinCashAddress(data) || HasSlpAddress(data) || HasCashTokensAddress(data)).ToArray(),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}