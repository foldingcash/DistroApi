namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiProvider : IStatsDownloadApiService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ILoggingService loggingService;

        private readonly IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService;

        private readonly IStatsDownloadApiDataStoreService statsDownloadApiDataStoreService;

        private readonly IStatsDownloadApiTokenDistributionService statsDownloadApiTokenDistributionService;

        public StatsDownloadApiProvider(IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService,
                                        IStatsDownloadApiTokenDistributionService
                                            statsDownloadApiTokenDistributionService, IDateTimeService dateTimeService,
                                        ILoggingService loggingService,
                                        IStatsDownloadApiDataStoreService statsDownloadApiDataStoreService)
        {
            this.statsDownloadApiDatabaseService = statsDownloadApiDatabaseService
                                                   ?? throw new ArgumentNullException(
                                                       nameof(statsDownloadApiDatabaseService));
            this.statsDownloadApiTokenDistributionService = statsDownloadApiTokenDistributionService
                                                            ?? throw new ArgumentNullException(
                                                                nameof(statsDownloadApiTokenDistributionService));
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.statsDownloadApiDataStoreService = statsDownloadApiDataStoreService
                                                    ?? throw new ArgumentNullException(
                                                        nameof(statsDownloadApiDataStoreService));
        }

        public GetDistroResponse GetDistro(DateTime? startDate, DateTime? endDate, int? amount)
        {
            loggingService.LogMethodInvoked();

            IList<ApiError> errors = new List<ApiError>();

            if (IsNotPreparedToRunDistro(startDate, endDate, amount, errors))
            {
                loggingService.LogMethodFinished();
                return new GetDistroResponse(errors);
            }

            IList<FoldingUser> foldingMembers = GetFoldingMembers(startDate, endDate);
            IList<DistroUser> distro = GetDistro(amount, foldingMembers);

            var distroResponse = new GetDistroResponse(distro);

            loggingService.LogMethodFinished();

            return distroResponse;
        }

        public GetMemberStatsResponse GetMemberStats(DateTime? startDate, DateTime? endDate)
        {
            loggingService.LogMethodInvoked();

            IList<ApiError> errors = new List<ApiError>();

            if (IsNotPreparedToGetMemberStats(startDate, endDate, errors))
            {
                loggingService.LogMethodFinished();
                return new GetMemberStatsResponse(errors);
            }

            DateTime startDateTime = startDate.GetValueOrDefault();
            DateTime endDateTime = endDate.GetValueOrDefault();

            if (startDateTime.Date == startDateTime && startDateTime.Date == endDateTime)
            {
                startDateTime = startDateTime.Date.AddHours(12);
                endDateTime = endDateTime.Date.AddHours(36);
            }

            IList<Member> members = statsDownloadApiDataStoreService.GetMembers(startDateTime, endDateTime);

            var memberStatsResponse = new GetMemberStatsResponse(members);

            loggingService.LogMethodFinished();

            return memberStatsResponse;
        }

        public GetTeamsResponse GetTeams()
        {
            loggingService.LogMethodInvoked();

            var errors = new List<ApiError>();

            if (IsNotPreparedToGetTeams(errors))
            {
                loggingService.LogMethodFinished();
                return new GetTeamsResponse(errors);
            }

            IList<Team> teams = statsDownloadApiDataStoreService.GetTeams();

            var teamsResponse = new GetTeamsResponse(teams);

            loggingService.LogMethodFinished();

            return teamsResponse;
        }

        private IList<DistroUser> GetDistro(int? amount, IList<FoldingUser> foldingUsers)
        {
            return statsDownloadApiTokenDistributionService.GetDistro(amount.GetValueOrDefault(), foldingUsers);
        }

        private IList<FoldingUser> GetFoldingMembers(DateTime? startDate, DateTime? endDate)
        {
            return statsDownloadApiDataStoreService.GetFoldingMembers(startDate.GetValueOrDefault(),
                endDate.GetValueOrDefault());
        }

        private bool IsNotPreparedToGetMemberStats(DateTime? startDate, DateTime? endDate, IList<ApiError> errors)
        {
            ValidateStartDate(startDate, errors);
            ValidateEndDate(endDate, errors);
            ValidateDateRange(startDate, endDate, errors);
            ValidateApiIsAvailable(errors);

            return errors.Count > 0;
        }

        private bool IsNotPreparedToGetTeams(IList<ApiError> errors)
        {
            ValidateApiIsAvailable(errors);

            return errors.Count > 0;
        }

        private bool IsNotPreparedToRunDistro(DateTime? startDate, DateTime? endDate, int? amount,
                                              IList<ApiError> errors)
        {
            ValidateStartDate(startDate, errors);
            ValidateEndDate(endDate, errors);
            ValidateDateRange(startDate, endDate, errors);
            ValidateAmount(amount, errors);
            ValidateApiIsAvailable(errors);

            return errors.Count > 0;
        }

        private void ValidateAmount(int? amount, IList<ApiError> errors)
        {
            if (amount == null)
            {
                errors.Add(Constants.ApiErrors.NoAmount);
                return;
            }

            if (amount == 0)
            {
                errors.Add(Constants.ApiErrors.ZeroAmount);
            }

            if (amount < 0)
            {
                errors.Add(Constants.ApiErrors.NegativeAmount);
            }
        }

        private void ValidateApiIsAvailable(IList<ApiError> errors)
        {
            ValidateDatabaseIsAvailable(errors);
            ValidateDataStoreIsAvailable(errors);
        }

        private void ValidateDatabaseIsAvailable(IList<ApiError> errors)
        {
            (bool isAvailable, DatabaseFailedReason reason) = statsDownloadApiDatabaseService.IsAvailable();

            if (!isAvailable)
            {
                if (reason == DatabaseFailedReason.DatabaseUnavailable)
                {
                    errors.Add(Constants.ApiErrors.DatabaseUnavailable);
                }
                else if (reason == DatabaseFailedReason.DatabaseMissingRequiredObjects)
                {
                    errors.Add(Constants.ApiErrors.DatabaseMissingRequiredObjects);
                }
            }
        }

        private void ValidateDataStoreIsAvailable(IList<ApiError> errors)
        {
            if (!statsDownloadApiDataStoreService.IsAvailable())
            {
                errors.Add(Constants.ApiErrors.DataStoreUnavailable);
            }
        }

        private void ValidateDate(DateTime? date, IList<ApiError> errors, ApiError noDate, ApiError dateUnsearchable)
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

        private void ValidateDateRange(DateTime? startDate, DateTime? endDate, IList<ApiError> errors)
        {
            if (startDate > endDate)
            {
                errors.Add(Constants.ApiErrors.InvalidDateRange);
            }
        }

        private void ValidateEndDate(DateTime? endDate, IList<ApiError> errors)
        {
            ValidateDate(endDate, errors, Constants.ApiErrors.NoEndDate, Constants.ApiErrors.EndDateUnsearchable);
        }

        private void ValidateStartDate(DateTime? startDate, IList<ApiError> errors)
        {
            ValidateDate(startDate, errors, Constants.ApiErrors.NoStartDate, Constants.ApiErrors.StartDateUnsearchable);
        }
    }
}