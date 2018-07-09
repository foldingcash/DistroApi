namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<DistroUser> GetDistroUsers();

        bool IsAvailable();
    }
}