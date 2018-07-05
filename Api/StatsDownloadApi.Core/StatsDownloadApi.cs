namespace StatsDownloadApi.Core
{
    using System.Collections.Generic;
    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApi : IStatsDownloadApi
    {
        private readonly IStatsDownloadDatabaseService databaseService;

        public StatsDownloadApi(IStatsDownloadDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public DistroResponse GetDistro()
        {
            IList<DistroError> errors = new List<DistroError>();

            if (!databaseService.IsAvailable())
            {
                AddDatabaseUnavailableError(errors);
                return new DistroResponse(errors);
            }

            return new DistroResponse();
        }

        private void AddDatabaseUnavailableError(IList<DistroError> errors)
        {
            errors.Add(Constants.DistroErrors.DatabaseUnavailable);
        }
    }
}