namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<FoldingUser> GetFoldingMembers(DateTime startDate, DateTime endDate);

        IList<Member> GetMembers(DateTime startDate, DateTime endDate);

        IList<Team> GetTeams();

        (bool isAvailable, DatabaseFailedReason reason) IsAvailable();
    }
}