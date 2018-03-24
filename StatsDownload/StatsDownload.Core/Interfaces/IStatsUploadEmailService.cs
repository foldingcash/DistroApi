namespace StatsDownload.Core
{
    using System.Collections.Generic;

    public interface IStatsUploadEmailService
    {
        void SendEmail(StatsUploadResult statsUploadResult);

        void SendEmail(List<FailedUserData> failedUsersData);
    }
}