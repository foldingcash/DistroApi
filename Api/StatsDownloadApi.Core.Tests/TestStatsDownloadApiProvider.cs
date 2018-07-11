namespace StatsDownloadApi.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Interfaces.DataTransfer;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsDownloadApiProvider
    {
        [SetUp]
        public void SetUp()
        {
            statsDownloadApiDatabaseServiceMock = Substitute.For<IStatsDownloadApiDatabaseService>();
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns(true);

            systemUnderTest = NewStatsDownloadApiProvider(statsDownloadApiDatabaseServiceMock);
        }

        private readonly DateTime endDate = DateTime.UtcNow;

        private readonly DateTime startDate = DateTime.UtcNow.Date;

        private IStatsDownloadApiDatabaseService statsDownloadApiDatabaseServiceMock;

        private IStatsDownloadApiService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiProvider(null));
        }

        [Test]
        public void GetDistro_WhenDatabaseIsUnavailable_ReturnsUnsuccessfulDistroResponse()
        {
            statsDownloadApiDatabaseServiceMock.IsAvailable().Returns(false);

            DistroResponse actual = InvokeGetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.DatabaseUnavailable));
            Assert.That(actual.DistroCount, Is.Null);
            Assert.That(actual.Distro, Is.Null);
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.DatabaseUnavailableMessage));
        }

        [Test]
        public void GetDistro_WhenInputIsInvalid_ReturnsUnsuccessfulDistroResponse()
        {
            DistroResponse actual = InvokeGetDistro(null, null);

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(2));
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.StartDateInvalid));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.StartDateInvalidMessage));
            Assert.That(actual.Errors?[1].ErrorCode, Is.EqualTo(DistroErrorCode.EndDateInvalid));
            Assert.That(actual.Errors?[1].ErrorMessage,
                Is.EqualTo(
                    Constants.ErrorMessages.EndDateInvalidMessage));
        }

        [Test]
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            var distro = new List<DistroUser>();
            statsDownloadApiDatabaseServiceMock.GetDistroUsers(startDate, endDate).Returns(distro);

            DistroResponse actual = InvokeGetDistro();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.None));
            Assert.That(actual.Distro, Is.EqualTo(distro));
        }

        private DistroResponse InvokeGetDistro()
        {
            return InvokeGetDistro(startDate, endDate);
        }

        private DistroResponse InvokeGetDistro(DateTime? startDate, DateTime? endDate)
        {
            return systemUnderTest.GetDistro(startDate, endDate);
        }

        private IStatsDownloadApiService NewStatsDownloadApiProvider(
            IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService)
        {
            return new StatsDownloadApiProvider(statsDownloadApiDatabaseService);
        }
    }
}