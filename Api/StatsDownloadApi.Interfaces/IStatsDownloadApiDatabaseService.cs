namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IStatsDownloadApiDatabaseService
    {
        [Obsolete]
        IList<FoldingUser> GetFoldingMembers(DateTime startDate, DateTime endDate);

        [Obsolete]
        IList<Member> GetMembers(DateTime startDate, DateTime endDate);

        [Obsolete]
        IList<Team> GetTeams();

        (bool isAvailable, DatabaseFailedReason reason) IsAvailable();
    }
}