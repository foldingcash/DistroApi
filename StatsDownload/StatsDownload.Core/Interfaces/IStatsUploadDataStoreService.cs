namespace StatsDownload.Core
{
    using System.Collections.Generic;

    public interface IStatsUploadDataStoreService
    {
        void AddUserData(UserData userData);

        List<int> GetDownloadsReadyForUpload();

        string GetFileData(int downloadId);

        bool IsAvailable();

        void StartStatsUpload(int downloadId);

        void StatsUploadFinished(int downloadId);
    }
}