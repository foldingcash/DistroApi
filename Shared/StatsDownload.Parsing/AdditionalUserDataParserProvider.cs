namespace StatsDownload.Parsing
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class AdditionalUserDataParserProvider : IAdditionalUserDataParserService
    {
        private readonly IBitcoinAddressValidatorService bitcoinAddressValidatorService;

        private readonly IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService;

        public AdditionalUserDataParserProvider(IBitcoinAddressValidatorService bitcoinAddressValidatorService,
                                                IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService)
        {
            this.bitcoinAddressValidatorService = bitcoinAddressValidatorService
                                                  ?? throw new ArgumentNullException(
                                                      nameof(bitcoinAddressValidatorService));
            this.bitcoinCashAddressValidatorService = bitcoinCashAddressValidatorService
                                                      ?? throw new ArgumentNullException(
                                                          nameof(bitcoinCashAddressValidatorService));
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

            if (bitcoinAddressValidatorService.IsValidBitcoinAddress(tokenizedName[addressPosition]))
            {
                userData.BitcoinAddress = tokenizedName[addressPosition];
            }
        }

        private void SetBitcoinCashAddress(string[] tokenizedName, UserData userData)
        {
            int addressPosition = tokenizedName.Length - 1;

            if (bitcoinCashAddressValidatorService.IsValidBitcoinCashAddress(tokenizedName[addressPosition]))
            {
                userData.BitcoinCashAddress = tokenizedName[addressPosition];
            }
        }

        private void SetFriendlyName(string[] tokenizedName, UserData userData)
        {
            if (tokenizedName.Length > 1 && (!string.IsNullOrWhiteSpace(userData.BitcoinAddress)
                                             || !string.IsNullOrWhiteSpace(userData.BitcoinCashAddress)))
            {
                userData.FriendlyName = tokenizedName[0];
            }
        }
    }
}