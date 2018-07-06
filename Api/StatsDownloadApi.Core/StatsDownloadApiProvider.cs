namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApiProvider : IStatsDownloadApiService
    {
        private readonly IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService;

        public StatsDownloadApiProvider(IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService)
        {
            this.statsDownloadApiDatabaseService = statsDownloadApiDatabaseService ??
                                                   throw new ArgumentNullException(
                                                       nameof(statsDownloadApiDatabaseService));
        }

        public DistroResponse GetDistro()
        {
            IList<DistroError> errors = new List<DistroError>();

            if (!statsDownloadApiDatabaseService.IsAvailable())
            {
                AddDatabaseUnavailableError(errors);
                return new DistroResponse(errors);
            }

            return new DistroResponse(statsDownloadApiDatabaseService.GetDistroUsers());
        }

        private void AddDatabaseUnavailableError(IList<DistroError> errors)
        {
            errors.Add(Constants.DistroErrors.DatabaseUnavailable);
        }
    }
}