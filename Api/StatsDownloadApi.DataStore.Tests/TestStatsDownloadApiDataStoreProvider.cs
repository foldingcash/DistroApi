namespace StatsDownloadApi.DataStore.Tests
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

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
                    BitcoinAddress = "btc1", FriendlyName = "user1", BitcoinCashAddress = null, SlpAddress = null,
                    CashTokensAddress = null
                },
                new UserData(2, "user2", 2, 2, 2)
                {
                    BitcoinAddress = null, FriendlyName = "user2", BitcoinCashAddress = "bch2", SlpAddress = null,
                    CashTokensAddress = null
                },
                new UserData(3, "user3", 3, 3, 3)
                {
                    BitcoinAddress = null, FriendlyName = "user3", BitcoinCashAddress = null, SlpAddress = "slp3",
                    CashTokensAddress = null
                },
                new UserData(4, "user4", 4, 4, 4)
                {
                    BitcoinAddress = null, FriendlyName = "user4", BitcoinCashAddress = null, SlpAddress = null,
                    CashTokensAddress = "tokens4"
                }
            };

            var followingUsersData = new[]
            {
                new UserData(1, "user1", 2, 2, 1)
                {
                    BitcoinAddress = "btc1", FriendlyName = "user1", BitcoinCashAddress = null, SlpAddress = null,
                    CashTokensAddress = null
                },
                new UserData(2, "user2", 4, 4, 2)
                {
                    BitcoinAddress = null, FriendlyName = "user2", BitcoinCashAddress = "bch2", SlpAddress = null,
                    CashTokensAddress = null
                },
                new UserData(3, "user3", 6, 6, 3)
                {
                    BitcoinAddress = null, FriendlyName = "user3", BitcoinCashAddress = null, SlpAddress = "slp3",
                    CashTokensAddress = null
                },
                new UserData(4, "user4", 8, 8, 4)
                {
                    BitcoinAddress = null, FriendlyName = "user4", BitcoinCashAddress = null, SlpAddress = null,
                    CashTokensAddress = "tokens4"
                },
                new UserData(5, "user5", 10, 10, 5)
                {
                    BitcoinAddress = null, FriendlyName = "user5", BitcoinCashAddress = null, SlpAddress = null,
                    CashTokensAddress = "tokens5"
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

            loggerMock = Substitute.For<ILogger<StatsDownloadApiDataStoreProvider>>();

            resourceCleanupServiceMock = Substitute.For<IResourceCleanupService>();

            systemUnderTest = new StatsDownloadApiDataStoreProvider(dataStoreServiceFactoryMock, databaseServiceMock,
                fileValidationServiceMock, filePayloadApiSettingsServiceMock, loggerMock,
                resourceCleanupServiceMock);
        }

        private IStatsDownloadApiDatabaseService databaseServiceMock;

        private IDataStoreService dataStoreServiceMock;

        private IFilePayloadApiSettingsService filePayloadApiSettingsServiceMock;

        private IFileValidationService fileValidationServiceMock;

        private ILogger<StatsDownloadApiDataStoreProvider> loggerMock;

        private IResourceCleanupService resourceCleanupServiceMock;

        private IStatsDownloadApiDataStoreService systemUnderTest;

        private readonly ValidatedFile validatedFileMock1 =
            new ValidatedFile(1, DateTime.Today.AddMinutes(1), "FilePath1");

        private readonly ValidatedFile validatedFileMock2 =
            new ValidatedFile(2, DateTime.Today.AddMinutes(2), "FilePath2");

        private readonly ValidatedFile validatedFileMock3 =
            new ValidatedFile(3, DateTime.Today.AddMinutes(3), "FilePath3");

        [Test]
        public async Task GetFoldingMembers_WhenInvoked_GetsAndValidatesStatsFiles()
        {
            await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue, FoldingUserTypes.All);

            databaseServiceMock.Received(1).GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);
            filePayloadApiSettingsServiceMock.Received(2).SetFilePayloadApiSettings(Arg.Any<FilePayload>());
            await dataStoreServiceMock.Received(1).DownloadFile(Arg.Any<FilePayload>(), validatedFileMock1);
            await dataStoreServiceMock.Received(1).DownloadFile(Arg.Any<FilePayload>(), validatedFileMock3);
            fileValidationServiceMock.Received(2).ValidateFile(Arg.Any<FilePayload>());
            resourceCleanupServiceMock.Received(2).Cleanup(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public async Task GetFoldingMembers_WhenInvoked_GetsFoldingMembers()
        {
            FoldingUsersResult result =
                await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue, FoldingUserTypes.All);
            FoldingUser[] actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(5));

            Assert.That(actual[0].FriendlyName, Is.EqualTo("user1"));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("btc1"));
            Assert.That(actual[0].BitcoinCashAddress, Is.Null);
            Assert.That(actual[0].SlpAddress, Is.Null);
            Assert.That(actual[0].CashTokensAddress, Is.Null);
            Assert.That(actual[0].PointsGained, Is.EqualTo(1));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(1));

            Assert.That(actual[1].FriendlyName, Is.EqualTo("user2"));
            Assert.That(actual[1].BitcoinAddress, Is.Null);
            Assert.That(actual[1].BitcoinCashAddress, Is.EqualTo("bch2"));
            Assert.That(actual[1].SlpAddress, Is.Null);
            Assert.That(actual[1].CashTokensAddress, Is.Null);
            Assert.That(actual[1].PointsGained, Is.EqualTo(2));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(2));

            Assert.That(actual[2].FriendlyName, Is.EqualTo("user3"));
            Assert.That(actual[2].BitcoinAddress, Is.Null);
            Assert.That(actual[2].BitcoinCashAddress, Is.Null);
            Assert.That(actual[2].SlpAddress, Is.EqualTo("slp3"));
            Assert.That(actual[2].CashTokensAddress, Is.Null);
            Assert.That(actual[2].PointsGained, Is.EqualTo(3));
            Assert.That(actual[2].WorkUnitsGained, Is.EqualTo(3));

            Assert.That(actual[3].FriendlyName, Is.EqualTo("user4"));
            Assert.That(actual[3].BitcoinAddress, Is.Null);
            Assert.That(actual[3].BitcoinCashAddress, Is.Null);
            Assert.That(actual[3].SlpAddress, Is.Null);
            Assert.That(actual[3].CashTokensAddress, Is.EqualTo("tokens4"));
            Assert.That(actual[3].PointsGained, Is.EqualTo(4));
            Assert.That(actual[3].WorkUnitsGained, Is.EqualTo(4));

            Assert.That(actual[4].FriendlyName, Is.EqualTo("user5"));
            Assert.That(actual[4].BitcoinAddress, Is.Null);
            Assert.That(actual[4].BitcoinCashAddress, Is.Null);
            Assert.That(actual[4].SlpAddress, Is.Null);
            Assert.That(actual[4].CashTokensAddress, Is.EqualTo("tokens5"));
            Assert.That(actual[4].PointsGained, Is.EqualTo(10));
            Assert.That(actual[4].WorkUnitsGained, Is.EqualTo(10));
        }

        [Test]
        public async Task GetFoldingMembers_WhenInvokedWithBitcoinCashType_GetsBitcoinCashFoldingMembers()
        {
            FoldingUsersResult result = await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue,
                FoldingUserTypes.BitcoinCash);
            FoldingUser[] actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0].BitcoinCashAddress, Is.EqualTo("bch2"));
        }

        [Test]
        public async Task GetFoldingMembers_WhenInvokedWithBitcoinType_GetsBitcoinFoldingMembers()
        {
            FoldingUsersResult result =
                await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue, FoldingUserTypes.Bitcoin);
            FoldingUser[] actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("btc1"));
        }

        [Test]
        public async Task GetFoldingMembers_WhenInvokedWithCashTokensType_GetsCashTokensFoldingMembers()
        {
            FoldingUsersResult result = await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue,
                FoldingUserTypes.CashTokens);
            FoldingUser[] actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(2));
            Assert.That(actual[0].CashTokensAddress, Is.EqualTo("tokens4"));
        }

        [TestCase(FoldingUserTypes.CashTokens | FoldingUserTypes.BitcoinCash, 3)]
        [TestCase(FoldingUserTypes.Bitcoin | FoldingUserTypes.BitcoinCash, 2)]
        [TestCase(FoldingUserTypes.Slp | FoldingUserTypes.BitcoinCash, 2)]
        [TestCase(FoldingUserTypes.CashTokens | FoldingUserTypes.BitcoinCash | FoldingUserTypes.Slp, 4)]
        public async Task GetFoldingMembers_WhenInvokedWithMultipleTypes_GetsMultiTypeFoldingMembers(
            FoldingUserTypes includeFoldingUserTypes, int expectedCount)
        {
            FoldingUsersResult result = await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue,
                includeFoldingUserTypes);
            FoldingUser[] actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetFoldingMembers_WhenInvokedWithSlpType_GetsSlpFoldingMembers()
        {
            FoldingUsersResult result =
                await systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue, FoldingUserTypes.Slp);
            FoldingUser[] actual = result.FoldingUsers;

            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0].SlpAddress, Is.EqualTo("slp3"));
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