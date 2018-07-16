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

            DistroResponse actual = InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.DatabaseUnavailableMessage));
        }

        [Test]
        public void GetDistro_WhenEndDateInputIsTodayOrFutureDate_ReturnsEndDateUnsearchableResponse()
        {
            DistroResponse actual = InvokeGetDistro(startDateMock, DateTime.UtcNow, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.EndDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.EndDateUnsearchableMessage));
        }

        [Test]
        public void GetDistro_WhenErrorsOccurs_ReturnsErrorResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns(false);

            DistroResponse actual = InvokeGetDistro(null, null, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(3));
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.NoStartDate));
            Assert.That(actual.DistroCount, Is.Null);
            Assert.That(actual.Distro, Is.Null);
        }

        [Test]
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            var foldingUsers = new DistroUser[1];
            var distro = new DistroUser[0];
            statsDownloadApiDatabaseServiceMock.GetFoldingUsers(startDateMock, endDateMock).Returns(foldingUsers);
            statsDownloadApiTokenDistributionServiceMock.GetDistro(foldingUsers).Returns(distro);

            DistroResponse actual = InvokeGetDistro();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.None));
            Assert.That(actual.Distro, Is.EqualTo(distro));
        }

        [Test]
        public void GetDistro_WhenNegativeAmountIsProvided_ReturnsNegativeAmountResponse()
        {
            DistroResponse actual = InvokeGetDistro(startDateMock, endDateMock, -1);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.NegativeAmount));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NegativeAmountMessage));
        }

        [Test]
        public void GetDistro_WhenNoAmountIsProvided_ReturnsNoAmountResponse()
        {
            DistroResponse actual = InvokeGetDistro(startDateMock, endDateMock, null);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.NoAmount));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NoAmountMessage));
        }

        [Test]
        public void GetDistro_WhenNoEndDateIsProvided_ReturnsNoEndDateResponse()
        {
            DistroResponse actual = InvokeGetDistro(startDateMock, null, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.NoEndDate));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NoEndDateMessage));
        }

        [Test]
        public void GetDistro_WhenNoStartDateIsProvided_ReturnsNoStartDateResponse()
        {
            DistroResponse actual = InvokeGetDistro(null, endDateMock, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.NoStartDate));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.NoStartDateMessage));
        }

        [Test]
        public void GetDistro_WhenStartDateInputIsTodayOrFutureDate_ReturnsStartDateUnsearchableResponse()
        {
            DistroResponse actual = InvokeGetDistro(DateTime.UtcNow, endDateMock, amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(2));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.StartDateUnsearchable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.StartDateUnsearchableMessage));
        }

        [Test]
        public void GetDistro_WhenStartDateIsLaterThanEndDate_ReturnsInvalidDateRangeResponse()
        {
            DistroResponse actual = InvokeGetDistro(startDateMock, endDateMock.AddDays(-1), amountMock);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.InvalidDateRange));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.InvalidDateRangeMessage));
        }

        [Test]
        public void GetDistro_WhenZeroAmountIsProvided_ReturnsZeroAmountResponse()
        {
            DistroResponse actual = InvokeGetDistro(startDateMock, endDateMock, 0);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.ZeroAmount));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.ZeroAmountMessage));
        }

        private DistroResponse InvokeGetDistro()
        {
            return InvokeGetDistro(startDateMock, endDateMock, amountMock);
        }

        private DistroResponse InvokeGetDistro(DateTime? startDate, DateTime? endDate, int? amount)
        {
            return systemUnderTest.GetDistro(startDate, endDate, amount);
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