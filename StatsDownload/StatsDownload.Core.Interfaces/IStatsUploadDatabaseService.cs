namespace StatsDownload.Core.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsUploadDatabaseService
    {
        void AddUsers(int downloadId, IEnumerable<UserData> usersData, IEnumerable<FailedUserData> failedUsers);

        IEnumerable<int> GetDownloadsReadyForUpload();

        string GetFileData(int downloadId);

        bool IsAvailable();

        void StartStatsUpload(int downloadId);

        void StatsUploadError(StatsUploadResult statsUploadResult);

        void StatsUploadFinished(int downloadId);
    }
}