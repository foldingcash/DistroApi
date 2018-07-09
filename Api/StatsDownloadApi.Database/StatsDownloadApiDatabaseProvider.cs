namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Interfaces;
    using Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApiDatabaseProvider : IStatsDownloadApiDatabaseService
    {
        private readonly IStatsDownloadDatabaseService statsDownloadDatabaseService;

        public StatsDownloadApiDatabaseProvider(IStatsDownloadDatabaseService statsDownloadDatabaseService)
        {
            this.statsDownloadDatabaseService = statsDownloadDatabaseService ??
                                                throw new ArgumentNullException(nameof(statsDownloadDatabaseService));
        }

        public IList<DistroUser> GetDistroUsers()
        {
            var users = new List<DistroUser>();
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(service =>
            {
                var dataTable = new DataTable();
                service.ExecuteStoredProcedure("[FoldingCoin].[GetDistroUsers]", dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    users.Add(new DistroUser(row["BitcoinAddress"] as string));
                }
            });
            return users;
        }

        public bool IsAvailable()
        {
            return statsDownloadDatabaseService.IsAvailable();
        }
    }
}