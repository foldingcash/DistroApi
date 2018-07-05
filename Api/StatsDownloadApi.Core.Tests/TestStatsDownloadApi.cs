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
        }

        [Test]
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            var actual = systemUnderTest.GetDistro();

            Assert.That(actual.Success, Is.True);
        }
    }
}