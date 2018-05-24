namespace StatsDownload.Core.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;
    using StatsDownload.Core.DataTransfer;

    public interface IStatsUploadEmailService
    {
        void SendEmail(StatsUploadResult statsUploadResult);

        void SendEmail(StatsUploadResults statsUploadResults);

        void SendEmail(IEnumerable<FailedUserData> failedUsersData);
    }
}