namespace StatsDownloadApi.Interfaces
{
    using System;

    [Flags]
    public enum FoldingUserTypes
    {
        Bitcoin = 1,

        BitcoinCash = 2,

        Slp = 4,

        CashTokens = 8,

        // Bitwise Or all the other flags to compose "All" enum
        All = Bitcoin | BitcoinCash | Slp | CashTokens,
    }
}