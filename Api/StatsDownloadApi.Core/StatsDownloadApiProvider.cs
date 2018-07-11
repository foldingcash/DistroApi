namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    public class StatsDownloadApiProvider : IStatsDownloadApiService
    {
        private readonly IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService;

        public StatsDownloadApiProvider(IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService)
        {
            this.statsDownloadApiDatabaseService = statsDownloadApiDatabaseService ??
                                                   throw new ArgumentNullException(
                                                       nameof(statsDownloadApiDatabaseService));
        }

        public DistroResponse GetDistro(DateTime? startDate, DateTime? endDate)
        {
            IList<DistroError> errors = new List<DistroError>();

            if (startDate == null)
            {
                errors.Add(Constants.DistroErrors.StartDateInvalid);
            }

            if (endDate == null)
            {
                errors.Add(Constants.DistroErrors.EndDateInvalid);
            }

            if (!statsDownloadApiDatabaseService.IsAvailable())
            {
                errors.Add(Constants.DistroErrors.DatabaseUnavailable);
            }

            if (errors.Count > 0)
            {
                return new DistroResponse(errors);
            }

            return new DistroResponse(statsDownloadApiDatabaseService.GetDistroUsers(startDate.Value, endDate.Value));
        }
    }
}