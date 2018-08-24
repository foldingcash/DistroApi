namespace StatsDownload.Core.Interfaces.Enums
{
    public enum RejectionReason
    {
        None,

        BitcoinAddressExceedsMaxSize,

        FahNameExceedsMaxSize,

        FailedAddToDatabase,

        FailedParsing,

        FriendlyNameExceedsMaxSize,

        UnexpectedFormat
    }
}