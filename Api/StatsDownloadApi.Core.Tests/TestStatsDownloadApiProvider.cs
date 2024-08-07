﻿namespace StatsDownloadApi.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NUnit.Framework;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Enums;

    [TestFixture]
    public class TestStatsDownloadApiProvider
    {
        [SetUp]
        public void SetUp()
        {
            loggerMock = Substitute.For<ILogger<StatsDownloadApiProvider>>();

            statsDownloadApiDatabaseServiceMock = Substitute.For<IStatsDownloadApiDatabaseService>();
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns((true, DatabaseFailedReason.None));

            statsDownloadApiTokenDistributionServiceMock = Substitute.For<IStatsDownloadApiTokenDistributionService>();

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(DateTime.UtcNow);

            statsDownloadApiDataStoreServiceMock = Substitute.For<IStatsDownloadApiDataStoreService>();
            statsDownloadApiDataStoreServiceMock.IsAvailable().Returns(true);

            systemUnderTest = NewStatsDownloadApiProvider(loggerMock, statsDownloadApiDatabaseServiceMock,
                statsDownloadApiTokenDistributionServiceMock, dateTimeServiceMock,
                statsDownloadApiDataStoreServiceMock);
        }

        private readonly int amountMock = 7750000;

        private IDateTimeService dateTimeServiceMock;

        private readonly DateTime endDateMock = DateTime.UtcNow.Date;

        private ILogger<StatsDownloadApiProvider> loggerMock;

        private readonly DateTime startDateMock = DateTime.UtcNow.Date;

        private IStatsDownloadApiDatabaseService statsDownloadApiDatabaseServiceMock;

        private IStatsDownloadApiDataStoreService statsDownloadApiDataStoreServiceMock;

        private IStatsDownloadApiTokenDistributionService statsDownloadApiTokenDistributionServiceMock;

        private IStatsDownloadApiService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiProvider(null,
                statsDownloadApiDatabaseServiceMock, statsDownloadApiTokenDistributionServiceMock, dateTimeServiceMock,
                statsDownloadApiDataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiProvider(loggerMock, null,
                statsDownloadApiTokenDistributionServiceMock, dateTimeServiceMock,
                statsDownloadApiDataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiProvider(loggerMock,
                statsDownloadApiDatabaseServiceMock, null, dateTimeServiceMock, statsDownloadApiDataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiProvider(loggerMock,
                statsDownloadApiDatabaseServiceMock, statsDownloadApiTokenDistributionServiceMock, null,
                statsDownloadApiDataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiProvider(loggerMock,
                statsDownloadApiDatabaseServiceMock, statsDownloadApiTokenDistributionServiceMock, dateTimeServiceMock,
                null));
        }

        [Test]
        public async Task GetDistro_WhenDatabaseIsUnavailable_ReturnsDatabaseUnavailableResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseUnavailable));

            GetDistroResponse actual = await InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.DatabaseUnavailableMessage));
        }

        [Test]
        public async Task GetDistro_WhenDatabaseMissingRequiredObjects_ReturnsDatabaseMissingRequiredObjectsResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseMissingRequiredObjects));

            GetDistroResponse actual = await InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseMissingRequiredObjects));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.DatabaseMissingRequiredObjectsMessage));
        }

        [Test]
        public async Task GetDistro_WhenDataStoreIsUnavailable_ReturnsDataStoreUnavailableResponse()
        {
            statsDownloadApiDataStoreServiceMock.IsAvailable().Returns(false);

            GetDistroResponse actual = await InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DataStoreUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.DataStoreUnavailableMessage));
        }

        [Test]
        public async Task GetDistro_WhenEndDateInputIsFutureDate_ReturnsEndDateUnsearchableResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(startDateMock, DateTime.UtcNow.AddDays(1), amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.EndDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.EndDateUnsearchableMessage));
        }

        [Test]
        public async Task GetDistro_WhenErrorsOccurs_ReturnsErrorResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseUnavailable));

            GetDistroResponse actual = await InvokeGetDistro(null, null, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(3));
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.NoStartDate));
            Assert.That(actual.DistroCount, Is.Null);
            Assert.That(actual.Distro, Is.Null);
            Assert.That(actual.TotalPoints, Is.Null);
            Assert.That(actual.TotalWorkUnits, Is.Null);
            Assert.That(actual.TotalDistro, Is.Null);
        }

        [Test]
        public async Task
            GetDistro_WhenInvalidDistributionStateExceptionThrown_ReturnsInvalidDistributionStateResponse()
        {
            var exception = new InvalidDistributionStateException("invalid state");

            var foldingUsers = new FoldingUser[0];
            var resultMock = new FoldingUsersResult(foldingUsers, startDateMock, endDateMock);
            statsDownloadApiDataStoreServiceMock.GetFoldingMembers(startDateMock, endDateMock, FoldingUserTypes.BitcoinCash).Returns(resultMock);
            statsDownloadApiTokenDistributionServiceMock.GetDistro(amountMock, foldingUsers)
                                                        .ThrowsForAnyArgs(exception);

            GetDistroResponse actual = await InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.InvalidDistributionState));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo("invalid state"));
        }

        [Test]
        public async Task GetDistro_WhenInvoked_ReturnsSuccessGetDistroResponse()
        {
            var foldingUsers = new FoldingUser[0];
            var distro = new[]
            {
                new DistroUser(null, null, null, null, 1, 2, 0.12345678m),
                new DistroUser(null, null, null, null, 3, 4, 100m)
            };
            var resultMock = new FoldingUsersResult(foldingUsers, startDateMock, endDateMock);
            statsDownloadApiDataStoreServiceMock.GetFoldingMembers(startDateMock, endDateMock, FoldingUserTypes.BitcoinCash).Returns(resultMock);
            statsDownloadApiTokenDistributionServiceMock.GetDistro(amountMock, foldingUsers).Returns(distro);

            GetDistroResponse actual = await InvokeGetDistro();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.None));
            Assert.That(actual.Distro, Is.EqualTo(distro));
            Assert.That(actual.TotalPoints, Is.EqualTo(4));
            Assert.That(actual.TotalWorkUnits, Is.EqualTo(6));
            Assert.That(actual.TotalDistro, Is.EqualTo(100.12345678));
            Assert.That(actual.StartDateTime, Is.EqualTo(startDateMock));
            Assert.That(actual.EndDateTime, Is.EqualTo(endDateMock));
        }

        [Test]
        public async Task GetDistro_WhenNegativeAmountIsProvided_ReturnsNegativeAmountResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(startDateMock, endDateMock, -1);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NegativeAmount));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.NegativeAmountMessage));
        }

        [Test]
        public async Task GetDistro_WhenNoAmountIsProvided_ReturnsNoAmountResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(startDateMock, endDateMock, null);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoAmount));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.NoAmountMessage));
        }

        [Test]
        public async Task GetDistro_WhenNoEndDateIsProvided_ReturnsNoEndDateResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(startDateMock, null, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoEndDate));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.NoEndDateMessage));
        }

        [Test]
        public async Task GetDistro_WhenNoStartDateIsProvided_ReturnsNoStartDateResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(null, endDateMock, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoStartDate));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.NoStartDateMessage));
        }

        [Test]
        public async Task GetDistro_WhenStartDateInputIsFutureDate_ReturnsStartDateUnsearchableResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(DateTime.UtcNow.AddDays(1), endDateMock, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(2));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.StartDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.StartDateUnsearchableMessage));
        }

        [Test]
        public async Task GetDistro_WhenStartDateIsLaterThanEndDate_ReturnsInvalidDateRangeResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(startDateMock, endDateMock.AddDays(-1), amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.InvalidDateRange));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.InvalidDateRangeMessage));
        }

        [Test]
        public async Task GetDistro_WhenZeroAmountIsProvided_ReturnsZeroAmountResponse()
        {
            GetDistroResponse actual = await InvokeGetDistro(startDateMock, endDateMock, 0);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.ZeroAmount));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.ZeroAmountMessage));
        }

        [Test]
        public async Task GetMembers_WhenDatabaseIsUnavailable_ReturnsDatabaseUnavailableResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseUnavailable));

            GetMembersResponse actual = await InvokeGetMembers();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.DatabaseUnavailableMessage));
        }

        [Test]
        public async Task
            GetMembers_WhenDatabaseMissingRequiredObjects_ReturnsDatabaseMissingRequiredObjectsResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseMissingRequiredObjects));

            GetMembersResponse actual = await InvokeGetMembers();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseMissingRequiredObjects));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.DatabaseMissingRequiredObjectsMessage));
        }

        [Test]
        public async Task GetMembers_WhenDataStoreIsUnavailable_ReturnsDataStoreUnavailableResponse()
        {
            statsDownloadApiDataStoreServiceMock.IsAvailable().Returns(false);

            GetMembersResponse actual = await InvokeGetMembers();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DataStoreUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.DataStoreUnavailableMessage));
        }

        [Test]
        public async Task GetMembers_WhenEndDateInputIsFutureDate_ReturnsEndDateUnsearchableResponse()
        {
            GetMembersResponse actual = await InvokeGetMembers(startDateMock, DateTime.UtcNow.AddDays(1));

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.EndDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.EndDateUnsearchableMessage));
        }

        [Test]
        public async Task GetMembers_WhenErrorsOccurs_ReturnsErrorsResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseUnavailable));

            GetMembersResponse actual = await InvokeGetMembers(null, null);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Members, Is.Null);
            Assert.That(actual.Errors?.Count, Is.EqualTo(3));
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.NoStartDate));
        }

        [Test]
        public async Task GetMembers_WhenInvoked_ReturnsSuccessMembersResponse()
        {
            var members = new Member[2];

            statsDownloadApiDataStoreServiceMock.GetMembers(DateTime.MinValue, endDateMock).Returns(members);

            GetMembersResponse actual = await InvokeGetMembers(DateTime.MinValue, endDateMock);

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.None));
            Assert.That(actual.Members, Is.EqualTo(members));
            Assert.That(actual.MemberCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetMembers_WhenNoEndDateIsProvided_ReturnsNoEndDateResponse()
        {
            GetMembersResponse actual = await InvokeGetMembers(startDateMock, null);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoEndDate));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.NoEndDateMessage));
        }

        [Test]
        public async Task GetMembers_WhenNoStartDateIsProvided_ReturnsNoStartDateResponse()
        {
            GetMembersResponse actual = await InvokeGetMembers(null, endDateMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoStartDate));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.NoStartDateMessage));
        }

        [Test]
        public async Task GetMembers_WhenStartDateInputIsFutureDate_ReturnsStartDateUnsearchableResponse()
        {
            GetMembersResponse actual = await InvokeGetMembers(DateTime.UtcNow.AddDays(1), endDateMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(2));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.StartDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.StartDateUnsearchableMessage));
        }

        [Test]
        public async Task GetMembers_WhenStartDateIsLaterThanEndDate_ReturnsInvalidDateRangeResponse()
        {
            GetMembersResponse actual = await InvokeGetMembers(startDateMock, endDateMock.AddDays(-1));

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.InvalidDateRange));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.InvalidDateRangeMessage));
        }

        [Test]
        public async Task GetTeams_WhenDatabaseIsUnavailable_ReturnsDatabaseUnavailableResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseUnavailable));

            GetTeamsResponse actual = await InvokeGetTeams();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage, Is.EqualTo(Constants.ErrorMessages.DatabaseUnavailableMessage));
        }

        [Test]
        public async Task GetTeams_WhenDatabaseMissingRequiredObjects_ReturnsDatabaseMissingRequiredObjectsResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable()
                                               .Returns((false, DatabaseFailedReason.DatabaseMissingRequiredObjects));

            GetTeamsResponse actual = await InvokeGetTeams();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseMissingRequiredObjects));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.DatabaseMissingRequiredObjectsMessage));
        }

        [Test]
        public async Task GetTeams_WhenDataStoreIsUnavailable_ReturnsDataStoreUnavailableResponse()
        {
            statsDownloadApiDataStoreServiceMock.IsAvailable().Returns(false);

            GetTeamsResponse actual = await InvokeGetTeams();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DataStoreUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(Constants.ErrorMessages.DataStoreUnavailableMessage));
        }

        [Test]
        public async Task GetTeams_WhenInvoked_ReturnsSuccessGetTeamsResponse()
        {
            var teams = new[] { new Team(0, ""), new Team(0, "") };
            statsDownloadApiDataStoreServiceMock.GetTeams().Returns(teams);

            GetTeamsResponse actual = await InvokeGetTeams();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.None));
            Assert.That(actual.Teams, Is.EqualTo(teams));
            Assert.That(actual.TeamCount, Is.EqualTo(2));
        }

        private async Task<GetDistroResponse> InvokeGetDistro()
        {
            return await InvokeGetDistro(startDateMock, endDateMock, amountMock);
        }

        private async Task<GetDistroResponse> InvokeGetDistro(DateTime? startDate, DateTime? endDate, int? amount)
        {
            return await systemUnderTest.GetDistro(startDate, endDate, amount, FoldingUserTypes.BitcoinCash);
        }

        private Task<GetMembersResponse> InvokeGetMembers(DateTime? startDate, DateTime? endDate)
        {
            return systemUnderTest.GetMembers(startDate, endDate);
        }

        private Task<GetMembersResponse> InvokeGetMembers()
        {
            return InvokeGetMembers(startDateMock, endDateMock);
        }

        private Task<GetTeamsResponse> InvokeGetTeams()
        {
            return systemUnderTest.GetTeams();
        }

        private IStatsDownloadApiService NewStatsDownloadApiProvider(ILogger<StatsDownloadApiProvider> logger,
            IStatsDownloadApiDatabaseService
                statsDownloadApiDatabaseService,
            IStatsDownloadApiTokenDistributionService
                statsDownloadApiTokenDistributionService,
            IDateTimeService dateTimeService,
            IStatsDownloadApiDataStoreService
                statsDownloadApiDataStoreService)
        {
            return new StatsDownloadApiProvider(logger, statsDownloadApiDatabaseService,
                statsDownloadApiTokenDistributionService, dateTimeService, statsDownloadApiDataStoreService);
        }
    }
}