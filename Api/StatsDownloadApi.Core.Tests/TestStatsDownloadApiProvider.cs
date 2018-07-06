namespace StatsDownloadApi.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

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

            DistroResponse actual = systemUnderTest.GetDistro();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Errors?.Count, Is.EqualTo(1));
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.DatabaseUnavailable));
            Assert.That(actual.DistroCount, Is.Null);
            Assert.That(actual.Distro, Is.Null);
            Assert.That(actual.Errors?[0].ErrorCode, Is.EqualTo(DistroErrorCode.DatabaseUnavailable));
            Assert.That(actual.Errors?[0].ErrorMessage,
                Is.EqualTo(
                    "The database is unavailable. Try again in a short period of time. If the problem continues, then contact the technical team."));
        }

        [Test] //TODO: May want to deprecate this as the functionality is completed
        public void GetDistro_WhenInvoked_GetsDistroUsersFromDatabase()
        {
            var distro = new List<DistroUser>();
            statsDownloadApiDatabaseServiceMock.GetDistroUsers().Returns(distro);

            DistroResponse actual = systemUnderTest.GetDistro();

            Assert.That(actual.Distro, Is.EqualTo(distro));
        }

        [Test]
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            DistroResponse actual = systemUnderTest.GetDistro();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.None));
            Assert.That(actual.DistroCount, Is.EqualTo(1));
            Assert.That(actual.Distro[0].BitcoinAddress, Is.EqualTo("address1"));
        }

        private IStatsDownloadApiService NewStatsDownloadApiProvider(
            IStatsDownloadApiDatabaseService statsDownloadApiDatabaseService)
        {
            return new StatsDownloadApiProvider(statsDownloadApiDatabaseService);
        }
    }
}