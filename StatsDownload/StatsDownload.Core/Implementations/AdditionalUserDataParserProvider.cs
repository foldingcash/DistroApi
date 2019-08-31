namespace StatsDownload.Core.Implementations
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class AdditionalUserDataParserProvider : IAdditionalUserDataParserService
    {
        private readonly IBitcoinAddressValidatorService bitcoinAddressValidatorService;

        public AdditionalUserDataParserProvider(IBitcoinAddressValidatorService bitcoinAddressValidatorService)
        {
            this.bitcoinAddressValidatorService = bitcoinAddressValidatorService
                                                  ?? throw new ArgumentNullException(
                                                      nameof(bitcoinAddressValidatorService));
        }

        public void Parse(UserData userData)
        {
            string[] tokenizedName = GetTokenizedName(userData);

            if (IsInvalidTokenLength(tokenizedName))
            {
                return;
            }

            SetBitcoinAddress(tokenizedName, userData);
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

        private void SetFriendlyName(string[] tokenizedName, UserData userData)
        {
            if (tokenizedName.Length > 1 && !string.IsNullOrWhiteSpace(userData.BitcoinAddress))
            {
                userData.FriendlyName = tokenizedName[0];
            }
        }
    }
}