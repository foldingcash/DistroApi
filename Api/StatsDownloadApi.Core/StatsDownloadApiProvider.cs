namespace StatsDownloadApi.Core
{
    using System.Collections.Generic;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class StatsDownloadApiProvider : IStatsDownloadApiService
    {
        private readonly IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService;

        public StatsDownloadApiProvider(IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService)
        {
            this.statsDownloadApiDatabaseService = statsDownloadApiDatabaseService;
        }

        public DistroResponse GetDistro()
        {
            IList<DistroError> errors = new List<DistroError>();

            if (!statsDownloadApiDatabaseService.IsAvailable())
            {
                AddDatabaseUnavailableError(errors);
                return new DistroResponse(errors);
            }

            IList<DistroUser> distro = new[] { new DistroUser("address1") };

            return new DistroResponse(distro);
        }

        private void AddDatabaseUnavailableError(IList<DistroError> errors)
        {
            errors.Add(Constants.DistroErrors.DatabaseUnavailable);
        }
    }
}