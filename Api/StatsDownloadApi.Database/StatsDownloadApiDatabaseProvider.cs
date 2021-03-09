namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Logging;

    using StatsDownloadApi.Interfaces;

    public class StatsDownloadApiDatabaseProvider : IStatsDownloadApiDatabaseService
    {
        private readonly ILogger<StatsDownloadApiDatabaseProvider> logger;

        private readonly IStatsDownloadDatabaseService statsDownloadDatabaseService;

        public StatsDownloadApiDatabaseProvider(ILogger<StatsDownloadApiDatabaseProvider> logger,
                                                IStatsDownloadDatabaseService statsDownloadDatabaseService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.statsDownloadDatabaseService = statsDownloadDatabaseService
                                                ?? throw new ArgumentNullException(
                                                    nameof(statsDownloadDatabaseService));
        }

        public IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            logger.LogMethodInvoked();

            var validatedFiles = new List<ValidatedFile>();
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(service =>
            {
                DbParameter startDateParameter =
                    service.CreateParameter("@StartDate", DbType.Date, ParameterDirection.Input);
                DbParameter endDateParameter =
                    service.CreateParameter("@EndDate", DbType.Date, ParameterDirection.Input);

                startDateParameter.Value = startDate;
                endDateParameter.Value = endDate;

                var dataTable = new DataTable();
                service.ExecuteStoredProcedure(Constants.StatsDownloadApiDatabase.GetValidatedFilesProcedureName,
                    new[] { startDateParameter, endDateParameter }, dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    validatedFiles.Add(new ValidatedFile((row["DownloadId"] as int?).GetValueOrDefault(),
                        (row["DownloadDateTime"] as DateTime?).GetValueOrDefault(), row["FilePath"] as string));
                }
            });
            return validatedFiles;
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable()
        {
            return statsDownloadDatabaseService.IsAvailable(Constants.StatsDownloadApiDatabase.ApiObjects);
        }
    }
}