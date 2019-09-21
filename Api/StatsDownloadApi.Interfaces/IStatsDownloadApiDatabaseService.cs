namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public interface IStatsDownloadApiDatabaseService
    {
        [Obsolete]
        IList<FoldingUser> GetFoldingMembers(DateTime startDate, DateTime endDate);

        [Obsolete]
        IList<Member> GetMembers(DateTime startDate, DateTime endDate);

        [Obsolete]
        IList<Team> GetTeams();

        IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate);

        (bool isAvailable, DatabaseFailedReason reason) IsAvailable();
    }
}