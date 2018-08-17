namespace StatsDownload.Core.Interfaces
{
    using System;
    using DataTransfer;
    using Enums;

    public interface IFileDownloadDatabaseService
    {
        void FileDownloadError(FileDownloadResult fileDownloadResult);

        void FileDownloadFinished(FilePayload filePayload);

        /// <summary>
        ///     Returns a DateTime of the last successful file download.
        ///     If there is no last successful download, should return DateTime.MinValue.
        /// </summary>
        /// <returns></returns>
        DateTime GetLastFileDownloadDateTime();

        (bool isAvailable, FailedReason reason) IsAvailable();

        void NewFileDownloadStarted(FilePayload filePayload);

        void UpdateToLatest();
    }
}