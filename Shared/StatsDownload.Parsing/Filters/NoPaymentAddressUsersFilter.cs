namespace StatsDownload.Parsing.Filters
{
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

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
                return new ParseResults(results.DownloadDateTime,
                    results.UsersData.Where(data => !string.IsNullOrWhiteSpace(data.BitcoinAddress)),
                    results.FailedUsersData);
            }

            return results;
        }
    }
}