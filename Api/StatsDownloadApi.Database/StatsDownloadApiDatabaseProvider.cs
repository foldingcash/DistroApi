namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public bool IsAvailable()
        {
            return statsDownloadDatabaseService.IsAvailable();
        }
    }
}