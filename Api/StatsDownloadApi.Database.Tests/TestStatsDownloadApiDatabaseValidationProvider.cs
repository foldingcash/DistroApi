namespace StatsDownloadApi.Database.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces;

    [TestFixture]
    public class TestStatsDownloadApiDatabaseValidationProvider
    {
        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsDownloadApiDatabaseService>();

            systemUnderTest = new StatsDownloadApiDatabaseValidationProvider(innerServiceMock);
        }

        private IStatsDownloadApiDatabaseService innerServiceMock;

        private IStatsDownloadApiDatabaseService systemUnderTest;

        [TestCase(true, DatabaseFailedReason.None)]
        [TestCase(false, DatabaseFailedReason.DatabaseUnavailable)]
        [TestCase(false, DatabaseFailedReason.DatabaseMissingRequiredObjects)]
        public void IsAvailable_WhenInvoked_DefersToInnerService(bool expectedIsAvailable,
                                                                 DatabaseFailedReason expectedReason)
        {
            innerServiceMock.IsAvailable().Returns((expectedIsAvailable, expectedReason));

            (bool isAvailable, DatabaseFailedReason reason) actual = systemUnderTest.IsAvailable();

            Assert.That(actual.isAvailable, Is.EqualTo(expectedIsAvailable));
            Assert.That(actual.reason, Is.EqualTo(expectedReason));
        }
    }
}