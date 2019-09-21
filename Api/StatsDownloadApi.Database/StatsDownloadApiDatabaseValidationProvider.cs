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
            throw new NotImplementedException();
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable()
        {
            return innerService.IsAvailable();
        }
    }
}