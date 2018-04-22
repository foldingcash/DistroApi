namespace StatsDownload.Core.Interfaces
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