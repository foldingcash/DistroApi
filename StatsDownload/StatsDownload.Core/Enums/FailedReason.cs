namespace StatsDownload.Core
{
    public enum FailedReason
    {
        None,

        DataStoreUnavailable,

        MinimumWaitTimeNotMet,

        RequiredSettingsInvalid,

        FileDownloadTimeout,

        FileDownloadFailedDecompression,

        UnexpectedException
    }
}