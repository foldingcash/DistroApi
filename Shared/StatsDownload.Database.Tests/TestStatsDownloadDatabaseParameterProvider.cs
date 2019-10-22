namespace StatsDownload.Database.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    [TestFixture]
    public class TestStatsDownloadDatabaseParameterProvider
    {
        [SetUp]
        public void SetUp()
        {
            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            systemUnderTest = NewStatsDownloadDatabaseParameterProvider(errorMessageServiceMock);

            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();
            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);
        }

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private IErrorMessageService errorMessageServiceMock;

        private IStatsDownloadDatabaseParameterService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadDatabaseParameterProvider(null));
        }

        [TestCase(ParameterDirection.Input)]
        [TestCase(ParameterDirection.Output)]
        public void CreateDownloadIdParameter_WhenDirectionProvided_CreatesDownloadIdParameterWithDirection(
            ParameterDirection expected)
        {
            DbParameter actual = systemUnderTest.CreateDownloadIdParameter(databaseConnectionServiceMock, expected);

            actual.Received().Direction = expected;
        }

        [Test]
        public void CreateDownloadIdParameter_WhenInvoked_CreatesInputDownloadIdParameter()
        {
            DbParameter actual = systemUnderTest.CreateDownloadIdParameter(databaseConnectionServiceMock);

            Assert.That(actual.ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
        }

        [Test]
        public void CreateDownloadIdParameter_WhenValueProvided_CreatesInputDownloadIdParameterWithValue()
        {
            DbParameter actual = systemUnderTest.CreateDownloadIdParameter(databaseConnectionServiceMock, 100);

            Assert.That(actual.ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actual.Value, Is.EqualTo(100));
        }

        [Test]
        public void
            CreateErrorMessageParameter_WhenInvokedWithFileDownloadResult_CreatesFileDownloadResultErrorMessageParameter()
        {
            var filePayload = new FilePayload();
            var fileDownloadResult = new FileDownloadResult(FailedReason.FileDownloadFailedDecompression, filePayload);

            errorMessageServiceMock
                .GetErrorMessage(fileDownloadResult.FailedReason, filePayload, StatsDownloadService.FileDownload)
                .Returns("Error Message");

            DbParameter actual =
                systemUnderTest.CreateErrorMessageParameter(databaseConnectionServiceMock, fileDownloadResult);

            Assert.That(actual.ParameterName, Is.EqualTo("@ErrorMessage"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.String));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actual.Value, Is.EqualTo("Error Message"));
        }

        [Test]
        public void
            CreateErrorMessageParameter_WhenInvokedWithStatsUploadResult_CreatesStatsUploadResultsErrorMessageParameter()
        {
            var statsUploadResult = new StatsUploadResult();

            errorMessageServiceMock.GetErrorMessage(statsUploadResult.FailedReason, StatsDownloadService.StatsUpload)
                                   .Returns("Error Message");

            DbParameter actual =
                systemUnderTest.CreateErrorMessageParameter(databaseConnectionServiceMock, statsUploadResult);

            Assert.That(actual.ParameterName, Is.EqualTo("@ErrorMessage"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.String));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actual.Value, Is.EqualTo("Error Message"));
        }

        [Test]
        public void CreateRejectionReasonParameter_WhenInvoked_CreatesRejectionReasonParameter()
        {
            DbParameter actual = systemUnderTest.CreateRejectionReasonParameter(databaseConnectionServiceMock);

            Assert.That(actual.ParameterName, Is.EqualTo("@RejectionReason"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.String));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
        }

        private IStatsDownloadDatabaseParameterService NewStatsDownloadDatabaseParameterProvider(
            IErrorMessageService errorMessageService)
        {
            return new StatsDownloadDatabaseParameterProvider(errorMessageService);
        }
    }
}