namespace StatsDownload.Core.Interfaces.Enums
{
    public enum FailedReason
    {
        None,

        DatabaseUnavailable,

        MissingRequiredObjects,

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