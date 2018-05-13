namespace StatsDownload.Core.Interfaces.Enums
{
    public enum FailedReason
    {
        None,

        DataStoreUnavailable,

        MinimumWaitTimeNotMet,

        RequiredSettingsInvalid,

        FileDownloadTimeout,

        FileDownloadFailedDecompression,

        InvalidStatsFileUpload,

        UnexpectedException
    }
}