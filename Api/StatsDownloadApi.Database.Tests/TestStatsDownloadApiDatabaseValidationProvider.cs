namespace StatsDownloadApi.Database.Tests
{
    using System;
    using System.Collections.Generic;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

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

        [Test]
        public void GetValidatedFiles_WhenInvoked_DefersToInnerService()
        {
            var expected = new[] { new ValidatedFile(), new ValidatedFile() };

            innerServiceMock.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue).Returns(expected);

            IList<ValidatedFile> actual = systemUnderTest.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetValidatedFiles_WhenNoResultsReturned_ThrowsNoAvailableStatsFilesException()
        {
            var expected = new ValidatedFile[0];

            innerServiceMock.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue).Returns(expected);

            Assert.Throws<NoAvailableStatsFilesException>(() =>
                systemUnderTest.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue));
        }

        [Test]
        public void GetValidatedFiles_WhenNullResultsReturned_ThrowsNoAvailableStatsFilesException()
        {
            var expected = new ValidatedFile[0];

            innerServiceMock.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue).Returns(expected);

            Assert.Throws<NoAvailableStatsFilesException>(() =>
                systemUnderTest.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue));
        }

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