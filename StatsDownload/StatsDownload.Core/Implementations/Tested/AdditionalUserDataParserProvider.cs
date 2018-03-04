namespace StatsDownload.Core
{
    using System;

    public class AdditionalUserDataParserProvider : IAdditionalUserDataParserService
    {
        private readonly IBitcoinAddressValidatorService bitcoinAddressValidatorService;

        public AdditionalUserDataParserProvider(IBitcoinAddressValidatorService bitcoinAddressValidatorService)
        {
            this.bitcoinAddressValidatorService = bitcoinAddressValidatorService;
        }

        public void Parse(UserData userData)
        {
            string name = userData.Name;
            string[] tokenizedName = name.Split(new[] { '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokenizedName.Length == 1)
            {
                if (bitcoinAddressValidatorService.IsValidBitcoinAddress(tokenizedName[0]))
                {
                    userData.BitcoinAddress = tokenizedName[0];
                }
            }

            if (tokenizedName.Length == 2)
            {
                userData.FriendlyName = tokenizedName[0];

                if (bitcoinAddressValidatorService.IsValidBitcoinAddress(tokenizedName[1]))
                {
                    userData.BitcoinAddress = tokenizedName[1];
                }
            }

            if (tokenizedName.Length == 3)
            {
                userData.FriendlyName = tokenizedName[0];

                if (bitcoinAddressValidatorService.IsValidBitcoinAddress(tokenizedName[2]))
                {
                    userData.BitcoinAddress = tokenizedName[2];
                }
            }
        }
    }
}