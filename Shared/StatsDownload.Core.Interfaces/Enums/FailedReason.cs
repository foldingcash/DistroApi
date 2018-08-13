namespace StatsDownload.Core.Interfaces.Enums
{
    public enum FailedReason
    {
        None,

        DatabaseUnavailable,

        MinimumWaitTimeNotMet,

        RequiredSettingsInvalid,

        FileDownloadTimeout,

        FileDownloadFailedDecompression,

        InvalidStatsFileUpload,

        UnexpectedDatabaseException,

        UnexpectedException,

        FileDownloadNotFound
    }
}