namespace StatsDownloadApi.DataStore.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;

    using StatsDownloadApi.Interfaces;

    [TestFixture]
    public class TestStatsDownloadApiDataStoreProvider
    {
        [SetUp]
        public void SetUp()
        {
            dataStoreServiceMock = Substitute.For<IDataStoreService>();

            systemUnderTest = new StatsDownloadApiDataStoreProvider(dataStoreServiceMock);
        }

        private IDataStoreService dataStoreServiceMock;

        private IStatsDownloadApiDataStoreService systemUnderTest;

        [TestCase(true)]
        [TestCase(false)]
        public void IsAvailable_WhenInvoked_DefersToDataStore(bool expected)
        {
            dataStoreServiceMock.IsAvailable().Returns(expected);

            bool actual = systemUnderTest.IsAvailable();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetFoldingMembers_WhenInvoked()
        {
        }

        [Test]
        public void GetMembers_WhenInvoked()
        {
        }

        [Test]
        public void GetTeams_WhenInvoked()
        {
        }
    }
}