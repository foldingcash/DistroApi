namespace StatsDownload.Parsing
{
    using System;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;

    public class AdditionalUserDataParserProvider : IAdditionalUserDataParserService
    {
        private readonly IBitcoinAddressValidatorService bitcoinAddressValidatorService;

        private readonly IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService;

        private readonly ICashTokensAddressValidatorService cashTokensAddressValidatorService;

        private readonly ISlpAddressValidatorService slpAddressValidatorService;

        public AdditionalUserDataParserProvider(IBitcoinAddressValidatorService bitcoinAddressValidatorService,
            IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService,
            ISlpAddressValidatorService slpAddressValidatorService,
            ICashTokensAddressValidatorService cashTokensAddressValidatorService)
        {
            this.bitcoinAddressValidatorService = bitcoinAddressValidatorService
                                                  ?? throw new ArgumentNullException(
                                                      nameof(bitcoinAddressValidatorService));
            this.bitcoinCashAddressValidatorService = bitcoinCashAddressValidatorService
                                                      ?? throw new ArgumentNullException(
                                                          nameof(bitcoinCashAddressValidatorService));
            this.slpAddressValidatorService = slpAddressValidatorService
                                              ?? throw new ArgumentNullException(nameof(slpAddressValidatorService));
            this.cashTokensAddressValidatorService = cashTokensAddressValidatorService ??
                                                     throw new ArgumentNullException(
                                                         nameof(cashTokensAddressValidatorService));
        }

        public void Parse(UserData userData)
        {
            string[] tokenizedName = GetTokenizedName(userData);

            if (IsInvalidTokenLength(tokenizedName))
            {
                return;
            }

            SetBitcoinAddress(tokenizedName, userData);
            SetBitcoinCashAddress(tokenizedName, userData);
            SetSlpAddress(tokenizedName, userData);
            SetCashTokensAddress(tokenizedName, userData);
            SetFriendlyName(tokenizedName, userData);
        }

        private string[] GetTokenizedName(UserData userData)
        {
            string name = userData.Name;
            return name?.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsInvalidTokenLength(string[] tokenizedName)
        {
            return tokenizedName == null || tokenizedName.Length < 1 || tokenizedName.Length > 3;
        }

        private void SetBitcoinAddress(string[] tokenizedName, UserData userData)
        {
            int addressPosition = tokenizedName.Length - 1;
            string address = tokenizedName[addressPosition];

            if (bitcoinAddressValidatorService.IsValidBitcoinAddress(address))
            {
                userData.BitcoinAddress = address;
            }
        }

        private void SetBitcoinCashAddress(string[] tokenizedName, UserData userData)
        {
            int addressPosition = tokenizedName.Length - 1;
            string address = tokenizedName[addressPosition];

            if (bitcoinCashAddressValidatorService.IsValidBitcoinCashAddress(address))
            {
                userData.BitcoinCashAddress = address;
            }
        }

        private void SetCashTokensAddress(string[] tokenizedName, UserData userData)
        {
            int addressPosition = tokenizedName.Length - 1;
            string address = tokenizedName[addressPosition];

            if (cashTokensAddressValidatorService.IsValidCashTokensAddress(address))
            {
                userData.CashTokensAddress = address;
            }
        }

        private void SetFriendlyName(string[] tokenizedName, UserData userData)
        {
            if (tokenizedName.Length > 1 && (!string.IsNullOrWhiteSpace(userData.BitcoinAddress)
                                             || !string.IsNullOrWhiteSpace(userData.BitcoinCashAddress)
                                             || !string.IsNullOrWhiteSpace(userData.SlpAddress)
                                             || !string.IsNullOrWhiteSpace(userData.CashTokensAddress)))
            {
                userData.FriendlyName = tokenizedName[0];
            }
        }

        private void SetSlpAddress(string[] tokenizedName, UserData userData)
        {
            int addressPosition = tokenizedName.Length - 1;
            string address = tokenizedName[addressPosition];

            if (slpAddressValidatorService.IsValidSlpAddress(address))
            {
                userData.SlpAddress = address;
            }
        }
    }
}