namespace StatsDownload.Core.Interfaces
{
    using System;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IFileDownloadDatabaseService
    {
        void FileDownloadError(FileDownloadResult fileDownloadResult);

        void FileDownloadFinished(FilePayload filePayload);

        void FileDownloadStarted(FilePayload filePayload);

        void FileValidated(FilePayload filePayload);

        void FileValidationError(FileDownloadResult fileDownloadResult);

        void FileValidationStarted(FilePayload filePayload);

        /// <summary>
        ///     Returns a DateTime of the last successful file download.
        ///     If there is no last successful download, should return DateTime.MinValue.
        /// </summary>
        /// <returns></returns>
        DateTime GetLastFileDownloadDateTime();

        (bool isAvailable, FailedReason reason) IsAvailable();

        void UpdateToLatest();
    }
}