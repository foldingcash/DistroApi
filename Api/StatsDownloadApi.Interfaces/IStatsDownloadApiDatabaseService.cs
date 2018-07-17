namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<FoldingUser> GetFoldingUsers(DateTime startDate, DateTime endDate);

        bool IsAvailable();
    }
}