namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public interface IStatsDownloadApiDatabaseService
    {
        IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate);

        (bool isAvailable, DatabaseFailedReason reason) IsAvailable();
    }
}