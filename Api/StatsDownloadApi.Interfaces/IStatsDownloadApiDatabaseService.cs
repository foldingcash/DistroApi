namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate);

        (bool isAvailable, DatabaseFailedReason reason) IsAvailable();
    }
}