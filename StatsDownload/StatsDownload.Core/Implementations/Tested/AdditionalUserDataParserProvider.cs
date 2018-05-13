namespace StatsDownload.Core.Implementations.Tested
{
    using System;

    using StatsDownload.Core.DataTransfer;
    using StatsDownload.Core.Interfaces;

    public class AdditionalUserDataParserProvider : IAdditionalUserDataParserService
    {
        private readonly IBitcoinAddressValidatorService bitcoinAddressValidatorService;

        public AdditionalUserDataParserProvider(IBitcoinAddressValidatorService bitcoinAddressValidatorService)
        {
            if (bitcoinAddressValidatorService == null)
            {
                throw new ArgumentNullException(nameof(bitcoinAddressValidatorService));
            }

            this.bitcoinAddressValidatorService = bitcoinAddressValidatorService;
        }

        public void Parse(UserData userData)
        {
            string[] tokenizedName = GetTokenizedName(userData);

            if (IsInvalidTokenLength(tokenizedName))
            {
                return;
            }

            SetFriendlyName(tokenizedName, userData);
            SetBitcoinAddress(tokenizedName, userData);
        }

        private string[] GetTokenizedName(UserData userData)
        {
            string name = userData.Name;
            return name.Split(new[] { '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsInvalidTokenLength(string[] tokenizedName)
        {
            return tokenizedName.Length < 1 || tokenizedName.Length > 3;
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
            if (tokenizedName.Length > 1)
            {
                userData.FriendlyName = tokenizedName[0];
            }
        }
    }
}