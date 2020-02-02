namespace StatsDownloadApi.DataStore.Tests
{
    using System;
    using System.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    [TestFixture]
    public class TestStatsDownloadApiDataStoreProvider
    {
        [SetUp]
        public void SetUp()
        {
            ValidatedFile[] validatedFiles = { validatedFileMock1, validatedFileMock2, validatedFileMock3 };

            var firstUsersData = new[]
                                 {
                                     new UserData(1, "user1", 1, 1, 1)
                                     {
                                         BitcoinAddress = "btc1", FriendlyName = "user1"
                                     },
                                     new UserData(2, "user2", 2, 2, 2)
                                     {
                                         BitcoinAddress = "btc2", FriendlyName = "user2"
                                     }
                                 };

            var followingUsersData = new[]
                                     {
                                         new UserData(1, "user1", 2, 2, 2)
                                         {
                                             BitcoinAddress = "btc1", FriendlyName = "user1"
                                         },
                                         new UserData(2, "user2", 4, 4, 4)
                                         {
                                             BitcoinAddress = "btc2", FriendlyName = "user2"
                                         }
                                     };

            dataStoreServiceMock = Substitute.For<IDataStoreService>();

            var dataStoreServiceFactoryMock = Substitute.For<IDataStoreServiceFactory>();
            dataStoreServiceFactoryMock.Create().Returns(dataStoreServiceMock);

            databaseServiceMock = Substitute.For<IStatsDownloadApiDatabaseService>();
            databaseServiceMock.GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue).Returns(validatedFiles);

            fileValidationServiceMock = Substitute.For<IFileValidationService>();
            fileValidationServiceMock.ValidateFile(Arg.Any<FilePayload>()).Returns(
                new ParseResults(DateTime.MinValue, firstUsersData, null),
                new ParseResults(DateTime.MaxValue, followingUsersData, null));

            filePayloadApiSettingsServiceMock = Substitute.For<IFilePayloadApiSettingsService>();

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = new StatsDownloadApiDataStoreProvider(dataStoreServiceFactoryMock, databaseServiceMock,
                fileValidationServiceMock, filePayloadApiSettingsServiceMock, loggingServiceMock);
        }

        private IStatsDownloadApiDatabaseService databaseServiceMock;

        private IDataStoreService dataStoreServiceMock;

        private IFilePayloadApiSettingsService filePayloadApiSettingsServiceMock;

        private IFileValidationService fileValidationServiceMock;

        private ILoggingService loggingServiceMock;

        private IStatsDownloadApiDataStoreService systemUnderTest;

        private readonly ValidatedFile validatedFileMock1 =
            new ValidatedFile(1, DateTime.Today.AddMinutes(1), "FilePath1");

        private readonly ValidatedFile validatedFileMock2 =
            new ValidatedFile(2, DateTime.Today.AddMinutes(2), "FilePath2");

        private readonly ValidatedFile validatedFileMock3 =
            new ValidatedFile(3, DateTime.Today.AddMinutes(3), "FilePath3");

        [Test]
        public void GetFoldingMembers_WhenInvoked_GetsAndValidatesStatsFiles()
        {
            systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue);

            Received.InOrder(() =>
            {
                databaseServiceMock.Received(1).GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);

                filePayloadApiSettingsServiceMock.Received(1).SetFilePayloadApiSettings(Arg.Any<FilePayload>());
                dataStoreServiceMock.Received(1).DownloadFile(Arg.Any<FilePayload>(), validatedFileMock1);
                fileValidationServiceMock.Received(1).ValidateFile(Arg.Any<FilePayload>());

                filePayloadApiSettingsServiceMock.Received(1).SetFilePayloadApiSettings(Arg.Any<FilePayload>());
                dataStoreServiceMock.Received(1).DownloadFile(Arg.Any<FilePayload>(), validatedFileMock3);
                fileValidationServiceMock.Received(1).ValidateFile(Arg.Any<FilePayload>());
            });
        }

        [Test]
        public async Task GetFoldingMembers_WhenInvoked_GetsFoldingMembers()
        {
            var result = await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue);
            var actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(2));

            Assert.That(actual[0].FriendlyName, Is.EqualTo("user1"));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("btc1"));
            Assert.That(actual[0].PointsGained, Is.EqualTo(1));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(1));

            Assert.That(actual[1].FriendlyName, Is.EqualTo("user2"));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("btc2"));
            Assert.That(actual[1].PointsGained, Is.EqualTo(2));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(2));
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
        public async Task IsAvailable_WhenInvoked_DefersToDataStore(bool expected)
        {
            dataStoreServiceMock.IsAvailable().Returns(expected);

            bool actual = await systemUnderTest.IsAvailable();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}