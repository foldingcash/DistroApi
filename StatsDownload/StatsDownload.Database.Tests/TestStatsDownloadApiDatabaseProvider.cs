namespace StatsDownload.Database.Tests
{
    using System;
    using Core.Interfaces;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsDownloadApiDatabaseProvider
    {
        [SetUp]
        public void SetUp()
        {
            statsDownloadDatabaseServiceMock = Substitute.For<IStatsDownloadDatabaseService>();

            systemUnderTest = NewStatsDownloadApiDatabaseProvider(statsDownloadDatabaseServiceMock);
        }

        private IStatsDownloadApiDatabaseService NewStatsDownloadApiDatabaseProvider(IStatsDownloadDatabaseService statsDownloadDatabaseService)
        {
            return new StatsDownloadApiDatabaseProvider(statsDownloadDatabaseService);
        }

        private IStatsDownloadDatabaseService statsDownloadDatabaseServiceMock;

        private IStatsDownloadApiDatabaseService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewStatsDownloadApiDatabaseProvider(null));
        }
    }
}