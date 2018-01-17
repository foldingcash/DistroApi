namespace StatsDownload.Core
{
    public enum FailedReason
    {
        None,

        DataStoreUnavailable,

        MinimumWaitTimeNotMet,

        FileDownloadTimeout,

        FileDownloadFailedDecompression,

        UnexpectedException
    }
}