namespace StatsDownloadApi.Core.Tests
{
    using NSubstitute;
    using NUnit.Framework;
    using StatsDownload.Core.Interfaces;

    [TestFixture]
    public class TestStatsDownloadApi
    {
        [SetUp]
        public void SetUp()
        {
            databaseServiceMock = Substitute.For<IStatsDownloadDatabaseService>();
            databaseServiceMock.IsAvailable().Returns(true);

            systemUnderTest = new StatsDownloadApi(databaseServiceMock);
        }

        private IStatsDownloadDatabaseService databaseServiceMock;

        private IStatsDownloadApi systemUnderTest;

        [Test]
        public void GetDistro_WhenDatabaseIsUnavailable_ReturnsUnsuccessfulDistroResponse()
        {
            databaseServiceMock.IsAvailable().Returns(false);

            var actual = systemUnderTest.GetDistro();

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

        [Test]
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            var actual = systemUnderTest.GetDistro();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.Errors, Is.Null);
            Assert.That(actual.ErrorCount, Is.Null);
            Assert.That(actual.FirstErrorCode, Is.EqualTo(DistroErrorCode.None));
            Assert.That(actual.DistroCount, Is.EqualTo(1));
            Assert.That(actual.Distro[0].BitcoinAddress, Is.EqualTo("address1"));
        }
    }
}