namespace StatsDownload.Core
{
    using System.Collections.Generic;

    public interface IStatsUploadDatabaseService
    {
        void AddUserData(int downloadId, UserData userData);

        List<int> GetDownloadsReadyForUpload();

        string GetFileData(int downloadId);

        bool IsAvailable();

        void StartStatsUpload(int downloadId);

        void StatsUploadError(StatsUploadResult statsUploadResult);

        void StatsUploadFinished(int downloadId);
    }
}