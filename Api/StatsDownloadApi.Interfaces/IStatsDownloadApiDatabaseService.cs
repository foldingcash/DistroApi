namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<DistroUser> GetDistroUsers(DateTime startDate, DateTime endDate);

        bool IsAvailable();
    }
}