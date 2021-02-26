namespace StatsDownload.Parsing
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class AdditionalUserDataParserProvider : IAdditionalUserDataParserService
    {
        private readonly IBitcoinAddressValidatorService bitcoinAddressValidatorService;

        private readonly IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService;

        private readonly ISlpAddressValidatorService slpAddressValidatorService;

        public AdditionalUserDataParserProvider(IBitcoinAddressValidatorService bitcoinAddressValidatorService,
                                                IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService,
                                                ISlpAddressValidatorService slpAddressValidatorService)
        {
            this.bitcoinAddressValidatorService = bitcoinAddressValidatorService
                                                  ?? throw new ArgumentNullException(
                                                      nameof(bitcoinAddressValidatorService));
            this.bitcoinCashAddressValidatorService = bitcoinCashAddressValidatorService
                                                      ?? throw new ArgumentNullException(
                                                          nameof(bitcoinCashAddressValidatorService));
            this.slpAddressValidatorService = slpAddressValidatorService
                                              ?? throw new ArgumentNullException(nameof(slpAddressValidatorService));
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
                                             || !string.IsNullOrWhiteSpace(userData.BitcoinCashAddress) || !string.IsNullOrWhiteSpace(userData.SlpAddress)))
            {
                userData.FriendlyName = tokenizedName[0];
            }
        }

        private void SetSlpAddress(string[] tokenizedName, UserData userData)
        {
            int addressPosition = tokenizedName.Length - 1;

            if (slpAddressValidatorService.IsValidSlpAddress(tokenizedName[addressPosition]))
            {
                userData.SlpAddress = tokenizedName[addressPosition];
            }
        }
    }
}