namespace StatsDownload.Core.Interfaces
{
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IStatsUploadDatabaseService
    {
        void AddUserRejection(int downloadId, FailedUserData failedUserData);

        void AddUsersData(int downloadId, IEnumerable<UserData> usersData);

        IEnumerable<int> GetDownloadsReadyForUpload();

        string GetFileData(int downloadId);

        bool IsAvailable();

        void StartStatsUpload(int downloadId);

        void StatsUploadError(StatsUploadResult statsUploadResult);

        void StatsUploadFinished(int downloadId);
    }
}