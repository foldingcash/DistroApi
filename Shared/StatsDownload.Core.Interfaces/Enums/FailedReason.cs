namespace StatsDownload.Core.Interfaces.Enums
{
    public enum FailedReason
    {
        None,

        DatabaseUnavailable,

        DatabaseMissingRequiredObjects,

        MinimumWaitTimeNotMet,

        RequiredSettingsInvalid,

        FileDownloadTimeout,

        FileDownloadNotFound,

        FileDownloadFailedDecompression,

        InvalidStatsFileUpload,

        UnexpectedDatabaseException,

        UnexpectedException,

        DataStoreUnavailable
    }
}