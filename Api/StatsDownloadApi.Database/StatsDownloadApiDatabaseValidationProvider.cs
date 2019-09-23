namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDatabaseValidationProvider : IStatsDownloadApiDatabaseService
    {
        private readonly IStatsDownloadApiDatabaseService innerService;

        public StatsDownloadApiDatabaseValidationProvider(IStatsDownloadApiDatabaseService innerService)
        {
            this.innerService = innerService;
        }

        public IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            IList<ValidatedFile> results = innerService.GetValidatedFiles(startDate, endDate);

            if (results is null || results.Count == 0)
            {
                throw new NoAvailableStatsFilesException();
            }

            return results;
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable()
        {
            return innerService.IsAvailable();
        }
    }
}