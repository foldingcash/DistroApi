namespace StatsDownloadApi.DataStore.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    [TestFixture]
    public class TestStatsDownloadApiDataStoreProvider
    {
        [SetUp]
        public void SetUp()
        {
            var validatedFiles = new[]
                                 {
                                     new ValidatedFile(1, DateTime.Today.AddMinutes(1), "FilePath1"),
                                     new ValidatedFile(2, DateTime.Today.AddMinutes(2), "FilePath2"),
                                     new ValidatedFile(3, DateTime.Today.AddMinutes(3), "FilePath3")
                                 };

            dataStoreServiceMock = Substitute.For<IDataStoreService>();

            databaseServiceMock = Substitute.For<IStatsDownloadApiDatabaseService>();
            databaseServiceMock.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue).Returns(validatedFiles);

            fileValidationServiceMock = Substitute.For<IFileValidationService>();

            filePayloadApiSettingsServiceMock = Substitute.For<IFilePayloadApiSettingsService>();

            systemUnderTest = new StatsDownloadApiDataStoreProvider(dataStoreServiceMock, databaseServiceMock,
                fileValidationServiceMock, filePayloadApiSettingsServiceMock);
        }

        private IStatsDownloadApiDatabaseService databaseServiceMock;

        private IDataStoreService dataStoreServiceMock;

        private IFilePayloadApiSettingsService filePayloadApiSettingsServiceMock;

        private IFileValidationService fileValidationServiceMock;

        private IStatsDownloadApiDataStoreService systemUnderTest;

        [Test]
        public void GetFoldingMembers_WhenInvoked_GetsFoldingMembers()
        {
            systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue);

            Received.InOrder(() =>
            {
                databaseServiceMock.Received(1).GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);

                filePayloadApiSettingsServiceMock.Received(1).SetFilePayloadApiSettings(Arg.Any<FilePayload>());
                dataStoreServiceMock.Received(1).DownloadFile(Arg.Any<FilePayload>());
                fileValidationServiceMock.Received(1).ValidateFile(Arg.Any<FilePayload>());

                filePayloadApiSettingsServiceMock.Received(1).SetFilePayloadApiSettings(Arg.Any<FilePayload>());
                dataStoreServiceMock.Received(1).DownloadFile(Arg.Any<FilePayload>());
                fileValidationServiceMock.Received(1).ValidateFile(Arg.Any<FilePayload>());
            });
        }

        [Test]
        public void GetMembers_WhenInvoked()
        {
        }

        [Test]
        public void GetTeams_WhenInvoked()
        {
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsAvailable_WhenInvoked_DefersToDataStore(bool expected)
        {
            dataStoreServiceMock.IsAvailable().Returns(expected);

            bool actual = systemUnderTest.IsAvailable();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}