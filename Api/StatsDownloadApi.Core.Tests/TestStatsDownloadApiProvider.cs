namespace StatsDownloadApi.Core.Tests
{
    using System;
    using Interfaces;
    using Interfaces.DataTransfer;
    using NSubstitute;
    using NUnit.Framework;
    using StatsDownload.Core.Interfaces;

    [TestFixture]
    public class TestStatsDownloadApiProvider
    {
        [SetUp]
        public void SetUp()
        {
            statsDownloadApiDatabaseServiceMock = Substitute.For<IStatsDownloadApiDatabaseService>();
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns(true);

            statsDownloadApiTokenDistributionServiceMock = Substitute.For<IStatsDownloadApiTokenDistributionService>();

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(DateTime.UtcNow);

            systemUnderTest = NewStatsDownloadApiProvider(statsDownloadApiDatabaseServiceMock,
                statsDownloadApiTokenDistributionServiceMock, dateTimeServiceMock);
        }

        private readonly int amountMock = 7750000;

        private IDateTimeService dateTimeServiceMock;

        private readonly DateTime endDateMock = DateTime.UtcNow.Date.AddDays(-1);

        private readonly DateTime startDateMock = DateTime.UtcNow.Date.AddDays(-1);

        private IStatsDownloadApiDatabaseService statsDownloadApiDatabaseServiceMock;

        private IStatsDownloadApiTokenDistributionService statsDownloadApiTokenDistributionServiceMock;

        private IStatsDownloadApiService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiProvider(null, statsDownloadApiTokenDistributionServiceMock, dateTimeServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiProvider(statsDownloadApiDatabaseServiceMock, null, dateTimeServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiProvider(statsDownloadApiDatabaseServiceMock,
                    statsDownloadApiTokenDistributionServiceMock, null));
        }

        [Test]
        public void GetDistro_WhenDatabaseIsUnavailable_ReturnsDatabaseUnavailableResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns(false);

            GetDistroResponse actual = InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.DatabaseUnavailableMessage));
        }

        [Test]
        public void GetDistro_WhenEndDateInputIsTodayOrFutureDate_ReturnsEndDateUnsearchableResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(startDateMock, DateTime.UtcNow, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.EndDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.EndDateUnsearchableMessage));
        }

        [Test]
        public void GetDistro_WhenErrorsOccurs_ReturnsErrorResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns(false);

            GetDistroResponse actual = InvokeGetDistro(null, null, amountMock);

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
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            var foldingUsers = new FoldingUser[0];
            var distro = new[]
            {
                new DistroUser(null, null, 1, 2, 0.12345678m),
                new DistroUser(null, null, 3, 4, 100m)
            };
            statsDownloadApiDatabaseServiceMock.GetFoldingUsers(startDateMock, endDateMock).Returns(foldingUsers);
            statsDownloadApiTokenDistributionServiceMock.GetDistro(amountMock, foldingUsers).Returns(distro);

            GetDistroResponse actual = InvokeGetDistro();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.None));
            Assert.That(actual.Distro, Is.EqualTo(distro));
            Assert.That(actual.TotalPoints, Is.EqualTo(4));
            Assert.That(actual.TotalWorkUnits, Is.EqualTo(6));
            Assert.That(actual.TotalDistro, Is.EqualTo(100.12345678));
        }

        [Test]
        public void GetDistro_WhenNegativeAmountIsProvided_ReturnsNegativeAmountResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(startDateMock, endDateMock, -1);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NegativeAmount));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NegativeAmountMessage));
        }

        [Test]
        public void GetDistro_WhenNoAmountIsProvided_ReturnsNoAmountResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(startDateMock, endDateMock, null);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoAmount));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NoAmountMessage));
        }

        [Test]
        public void GetDistro_WhenNoEndDateIsProvided_ReturnsNoEndDateResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(startDateMock, null, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoEndDate));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NoEndDateMessage));
        }

        [Test]
        public void GetDistro_WhenNoStartDateIsProvided_ReturnsNoStartDateResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(null, endDateMock, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.NoStartDate));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NoStartDateMessage));
        }

        [Test]
        public void GetDistro_WhenStartDateInputIsTodayOrFutureDate_ReturnsStartDateUnsearchableResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(DateTime.UtcNow, endDateMock, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(2));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.StartDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.StartDateUnsearchableMessage));
        }

        [Test]
        public void GetDistro_WhenStartDateIsLaterThanEndDate_ReturnsInvalidDateRangeResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(startDateMock, endDateMock.AddDays(-1), amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.InvalidDateRange));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.InvalidDateRangeMessage));
        }

        [Test]
        public void GetDistro_WhenZeroAmountIsProvided_ReturnsZeroAmountResponse()
        {
            GetDistroResponse actual = InvokeGetDistro(startDateMock, endDateMock, 0);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(ApiErrorCode.ZeroAmount));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.ZeroAmountMessage));
        }

        [Test]
        public void GetTeams_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            GetTeamsResponse actual = InvokeGetTeams();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(ApiErrorCode.None));
            Assert.That(actual.TeamCount, Is.EqualTo(2));

            Assert.That(actual.Teams[0].TeamNumber, Is.EqualTo(1234));
            Assert.That(actual.Teams[0].TeamName, Is.EqualTo("1234"));
            Assert.That(actual.Teams[1].TeamNumber, Is.EqualTo(2345));
            Assert.That(actual.Teams[1].TeamName, Is.EqualTo("2345"));
        }

        private GetDistroResponse InvokeGetDistro()
        {
            return InvokeGetDistro(startDateMock, endDateMock, amountMock);
        }

        private GetDistroResponse InvokeGetDistro(DateTime? startDate, DateTime? endDate, int? amount)
        {
            return systemUnderTest.GetDistro(startDate, endDate, amount);
        }

        private GetTeamsResponse InvokeGetTeams()
        {
            return systemUnderTest.GetTeams();
        }

        private IStatsDownloadApiService NewStatsDownloadApiProvider(
            IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService,
            IStatsDownloadApiTokenDistributionService statsDownloadApiTokenDistributionService,
            IDateTimeService dateTimeService)
        {
            return new StatsDownloadApiProvider(statsDownloadApiDatabaseService,
                statsDownloadApiTokenDistributionService, dateTimeService);
        }
    }
}