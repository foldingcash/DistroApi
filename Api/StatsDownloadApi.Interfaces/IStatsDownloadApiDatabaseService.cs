namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<FoldingUser> GetFoldingUsers(DateTime startDate, DateTime endDate);

        IList<Member> GetMembers(DateTime startDate, DateTime endDate);

        IList<Team> GetTeams();

        bool IsAvailable();
    }
}