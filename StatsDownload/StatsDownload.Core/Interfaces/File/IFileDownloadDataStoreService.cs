namespace StatsDownload.Core
{
    using System;
    using Interfaces;

    public interface IFileDownloadDataStoreService
    {
        void FileDownloadError(FileDownloadResult fileDownloadResult);

        void FileDownloadFinished(FilePayload filePayload);

        /// <summary>
        ///     Returns a DateTime of the last successful file download.
        ///     If there is no last successful download, should return DateTime.MinValue.
        /// </summary>
        /// <returns></returns>
        DateTime GetLastFileDownloadDateTime();

        bool IsAvailable();

        void NewFileDownloadStarted(FilePayload filePayload);

        void UpdateToLatest();
    }
}