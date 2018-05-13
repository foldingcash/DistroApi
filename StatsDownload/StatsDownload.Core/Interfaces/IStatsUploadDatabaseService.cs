namespace StatsDownload.Core.Interfaces
{
    using System.Collections.Generic;

    using StatsDownload.Core.DataTransfer;

    public interface IStatsUploadDatabaseService
    {
        void AddUserData(int downloadId, UserData userData);

        void AddUserRejection(int downloadId, FailedUserData failedUserData);

        IEnumerable<int> GetDownloadsReadyForUpload();

        string GetFileData(int downloadId);

        bool IsAvailable();

        void StartStatsUpload(int downloadId);

        void StatsUploadError(StatsUploadResult statsUploadResult);

        void StatsUploadFinished(int downloadId);
    }
}