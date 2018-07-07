namespace StatsDownload.Database
{
    using System;
    using System.Collections.Generic;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;

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
            throw new NotImplementedException();
        }
    }
}