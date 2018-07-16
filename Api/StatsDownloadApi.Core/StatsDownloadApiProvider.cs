namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApiProvider : IStatsDownloadApiService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService;

        private readonly IStatsDownloadApiTokenDistributionService statsDownloadApiTokenDistributionService;

        public StatsDownloadApiProvider(IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService,
            IStatsDownloadApiTokenDistributionService statsDownloadApiTokenDistributionService,
            IDateTimeService dateTimeService)
        {
            this.statsDownloadApiDatabaseService = statsDownloadApiDatabaseService ??
                                                   throw new ArgumentNullException(
                                                       nameof(statsDownloadApiDatabaseService));
            this.statsDownloadApiTokenDistributionService = statsDownloadApiTokenDistributionService ??
                                                            throw new ArgumentNullException(
                                                                nameof(statsDownloadApiTokenDistributionService));
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        }

        public DistroResponse GetDistro(DateTime? startDate, DateTime? endDate, int? amount)
        {
            IList<DistroError> errors = new List<DistroError>();

            if (IsNotPreparedToRunDistro(startDate, endDate, amount, errors))
            {
                return new DistroResponse(errors);
            }

            IList<DistroUser> foldingUsers = GetFoldingUsers(startDate, endDate);
            IList<DistroUser> distro = GetDistro(amount, foldingUsers);

            return new DistroResponse(distro);
        }

        private IList<DistroUser> GetDistro(int? amount, IList<DistroUser> foldingUsers)
        {
            return statsDownloadApiTokenDistributionService.GetDistro(amount.GetValueOrDefault(), foldingUsers);
        }

        private IList<DistroUser> GetFoldingUsers(DateTime? startDate, DateTime? endDate)
        {
            return statsDownloadApiDatabaseService.GetFoldingUsers(startDate.GetValueOrDefault(),
                endDate.GetValueOrDefault());
        }

        private bool IsNotPreparedToRunDistro(DateTime? startDate, DateTime? endDate, int? amount,
            IList<DistroError> errors)
        {
            ValidateStartDate(startDate, errors);
            ValidateEndDate(endDate, errors);
            ValidateDateRange(startDate, endDate, errors);
            ValidateAmount(amount, errors);
            ValidateDatabaseIsAvailable(errors);

            return errors.Count > 0;
        }

        private void ValidateAmount(int? amount, IList<DistroError> errors)
        {
            if (amount == null)
            {
                errors.Add(Constants.DistroErrors.NoAmount);
                return;
            }

            if (amount == 0)
            {
                errors.Add(Constants.DistroErrors.ZeroAmount);
            }

            if (amount < 0)
            {
                errors.Add(Constants.DistroErrors.NegativeAmount);
            }
        }

        private void ValidateDatabaseIsAvailable(IList<DistroError> errors)
        {
            if (!statsDownloadApiDatabaseService.IsAvailable())
            {
                errors.Add(Constants.DistroErrors.DatabaseUnavailable);
            }
        }

        private void ValidateDate(DateTime? date, IList<DistroError> errors, DistroError noDate,
            DistroError dateUnsearchable)
        {
            if (date == null)
            {
                errors.Add(noDate);
                return;
            }

            if (date.Value.Date >= dateTimeService.DateTimeNow().Date)
            {
                errors.Add(dateUnsearchable);
            }
        }

        private void ValidateDateRange(DateTime? startDate, DateTime? endDate, IList<DistroError> errors)
        {
            if (startDate > endDate)
            {
                errors.Add(Constants.DistroErrors.InvalidDateRange);
            }
        }

        private void ValidateEndDate(DateTime? endDate, IList<DistroError> errors)
        {
            ValidateDate(endDate, errors, Constants.DistroErrors.NoEndDate, Constants.DistroErrors.EndDateUnsearchable);
        }

        private void ValidateStartDate(DateTime? startDate, IList<DistroError> errors)
        {
            ValidateDate(startDate, errors, Constants.DistroErrors.NoStartDate,
                Constants.DistroErrors.StartDateUnsearchable);
        }
    }
}