namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using NSubstitute;
    using NSubstitute.ClearExtensions;

    using NUnit.Framework;

    using StatsDownload.Logging;

    [TestFixture]
    public class TestStatsDownloadDatabaseProvider
    {
        private const int NumberOfRowsEffectedExpected = 5;

        private IDatabaseConnectionServiceFactory databaseConnectionServiceFactoryMock;

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private IDatabaseConnectionSettingsService databaseConnectionSettingsServiceMock;

        private DbDataReader dbDataReaderMock;

        private IErrorMessageService errorMessageServiceMock;

        private FileDownloadResult fileDownloadResult;

        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private IStatsDownloadDatabaseService systemUnderTest;

        [Test]
        public void AddUserData_WhenInvoked_ParameterIsProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service => service.ExecuteStoredProcedure("[FoldingCoin].[AddUserData]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            systemUnderTest.AddUserData(1,
                new UserData("name", 10, 100, 1000) { BitcoinAddress = "address", FriendlyName = "friendly" });

            Assert.That(actualParameters.Count, Is.EqualTo(7));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(1));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@FAHUserName"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo("name"));
            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@TotalPoints"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.Int64));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[2].Value, Is.EqualTo(10));
            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@WorkUnits"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.Int64));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[3].Value, Is.EqualTo(100));
            Assert.That(actualParameters[4].ParameterName, Is.EqualTo("@TeamNumber"));
            Assert.That(actualParameters[4].DbType, Is.EqualTo(DbType.Int64));
            Assert.That(actualParameters[4].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[4].Value, Is.EqualTo(1000));
            Assert.That(actualParameters[5].ParameterName, Is.EqualTo("@FriendlyName"));
            Assert.That(actualParameters[5].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[5].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[5].Value, Is.EqualTo("friendly"));
            Assert.That(actualParameters[6].ParameterName, Is.EqualTo("@BitcoinAddress"));
            Assert.That(actualParameters[6].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[6].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[6].Value, Is.EqualTo("address"));
        }

        [Test]
        public void AddUserData_WhenInvoked_StartsStatsUpload()
        {
            systemUnderTest.AddUserData(1, new UserData());

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("AddUserData Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[AddUserData]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(null, databaseConnectionServiceFactoryMock, loggingServiceMock,
                    errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock, null, loggingServiceMock,
                    errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock,
                    databaseConnectionServiceFactoryMock, null, errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock,
                    databaseConnectionServiceFactoryMock, loggingServiceMock, null));
        }

        [Test]
        public void FileDownloadError_WhenInvoked_ParametersAreProvided()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, filePayload)
                                   .Returns("ErrorMessage");
            fileDownloadResult = new FileDownloadResult(FailedReason.UnexpectedException, filePayload);

            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadError]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeFileDownloadError();

            Assert.That(actualParameters.Count, Is.EqualTo(2));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(100));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@ErrorMessage"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo("ErrorMessage"));
        }

        [Test]
        public void FileDownloadError_WhenInvoked_UpdatesFileDownloadToError()
        {
            InvokeFileDownloadError();

            Received.InOrder((() =>
            {
                loggingServiceMock.LogVerbose("FileDownloadError Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadError]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            }));
        }

        [Test]
        public void FileDownloadFinished_WhenInvoked_FileDataUpload()
        {
            InvokeFileDownloadFinished();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("FileDownloadFinished Invoked");
                databaseConnectionServiceMock.Open();
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadFinished]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void FileDownloadFinished_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadFinished]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeFileDownloadFinished();

            Assert.That(actualParameters.Count, Is.EqualTo(4));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(100));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@FileName"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo("DecompressedDownloadFileName"));
            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@FileExtension"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[2].Value, Is.EqualTo("DecompressedDownloadFileExtension"));
            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@FileData"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[3].Value, Is.EqualTo("DecompressedDownloadFileData"));
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenInvoked_GetsDownloadsReadyForUpload()
        {
            systemUnderTest.GetDownloadsReadyForUpload();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("GetDownloadsReadyForUpload Invoked");
                databaseConnectionServiceMock.Open();
                databaseConnectionServiceMock.ExecuteReader(
                    "SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload]");
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenInvoked_ReturnsDownloadIds()
        {
            List<int> actual = systemUnderTest.GetDownloadsReadyForUpload();

            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual[1], Is.EqualTo(200));
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenNoFilesReadyForUpload_ReturnsEmptyList()
        {
            dbDataReaderMock.ClearSubstitute();
            dbDataReaderMock.Read().Returns(false);

            List<int> actual = systemUnderTest.GetDownloadsReadyForUpload();

            Assert.That(actual.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetFileData_WhenInvoked_GetFileData()
        {
            systemUnderTest.GetFileData(100);

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("GetFileData Invoked");
                databaseConnectionServiceMock.Open();
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[GetFileData]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void GetFileData_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service => service.ExecuteStoredProcedure("[FoldingCoin].[GetFileData]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            systemUnderTest.GetFileData(100);

            Assert.That(actualParameters.Count, Is.EqualTo(4));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(100));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@FileName"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(actualParameters[1].Size, Is.EqualTo(-1));
            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@FileExtension"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(actualParameters[2].Size, Is.EqualTo(-1));
            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@FileData"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(actualParameters[3].Size, Is.EqualTo(-1));
        }

        [Test]
        public void GetFileData_WhenInvoked_ReturnsFileData()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns("FileData");

            databaseConnectionServiceMock.ClearSubstitute();
            databaseConnectionServiceMock.CreateParameter("@FileData", DbType.String, ParameterDirection.Output, -1)
                                         .Returns(dbParameter);
            databaseConnectionServiceMock.CreateParameter("@DownloadId", DbType.Int32, ParameterDirection.Input)
                                         .Returns(Substitute.For<DbParameter>());

            string actual = systemUnderTest.GetFileData(100);

            Assert.That(actual, Is.EqualTo("FileData"));
        }

        [Test]
        public void GetLastFileDownloadDateTime_WhenInvoked_GetsLastfileDownloadDateTime()
        {
            InvokeGetLastFileFownloadDateTime();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("GetLastFileDownloadDateTime Invoked");
                databaseConnectionServiceMock.Open();
                databaseConnectionServiceMock.ExecuteScalar("SELECT [FoldingCoin].[GetLastFileDownloadDateTime]()");
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void GetLastFileDownloadDateTime_WhenInvoked_ReturnsDateTime()
        {
            DateTime dateTime = DateTime.Now;
            databaseConnectionServiceMock.ExecuteScalar("SELECT [FoldingCoin].[GetLastFileDownloadDateTime]()")
                                         .Returns(dateTime);

            DateTime actual = InvokeGetLastFileFownloadDateTime();

            Assert.That(actual, Is.EqualTo(dateTime));
        }

        [Test]
        public void GetLastFileDownloadDateTime_WhenNoRowsReturned_ReturnsDefaultDateTime()
        {
            DateTime actual = InvokeGetLastFileFownloadDateTime();

            Assert.That(actual, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void IsAvailable_Invoked_ConnectionOpenedThenClosed()
        {
            InvokeIsAvailable();

            Received.InOrder((() =>
            {
                loggingServiceMock.LogVerbose("IsAvailable Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.Close();
            }));
        }

        [Test]
        public void IsAvailable_WhenDatabaseConnectionFails_ConnectionOpenedThenClosed()
        {
            var expected = new Exception();
            databaseConnectionServiceMock.When(mock => mock.Open()).Throw(expected);

            InvokeIsAvailable();

            Received.InOrder((() =>
            {
                loggingServiceMock.LogVerbose("IsAvailable Invoked");
                databaseConnectionServiceMock.Open();
                databaseConnectionServiceMock.Close();
                loggingServiceMock.LogException(expected);
            }));
        }

        [Test]
        public void IsAvailable_WhenDatabaseConnectionFails_ReturnsFalse()
        {
            databaseConnectionServiceMock.When(mock => mock.Open()).Throw<Exception>();

            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsAvailable_WhenInvalidConnectionString_ReturnsFalse(string connectionString)
        {
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns(connectionString);

            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsAvailable_WhenInvoked_ReturnsTrue()
        {
            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.True);
        }

        [Test]
        public void NewFileDownloadStarted_WhenEmptyString_ThrowsArgumentException()
        {
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns(string.Empty);

            Assert.Throws<ArgumentException>(InvokeNewFileDownloadStarted);
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_NewFileDownloadStarted()
        {
            InvokeNewFileDownloadStarted();

            Received.InOrder((() =>
            {
                loggingServiceMock.LogVerbose("NewFileDownloadStarted Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[NewFileDownloadStarted]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            }));
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[NewFileDownloadStarted]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeNewFileDownloadStarted();

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Output));
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_ReturnsDownloadId()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns(101);

            databaseConnectionServiceMock.ClearSubstitute();
            databaseConnectionServiceMock.CreateParameter("@DownloadId", DbType.Int32, ParameterDirection.Output)
                                         .Returns(dbParameter);

            InvokeNewFileDownloadStarted();

            Assert.That(filePayload.DownloadId, Is.EqualTo(101));
        }

        [Test]
        public void NewFileDownloadStarted_WhenNullConnectionString_ThrowsArgumentNullException()
        {
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns((string)null);

            Assert.Throws<ArgumentNullException>(InvokeNewFileDownloadStarted);
        }

        [SetUp]
        public void SetUp()
        {
            databaseConnectionSettingsServiceMock = Substitute.For<IDatabaseConnectionSettingsService>();
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns("connectionString");

            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();
            databaseConnectionServiceFactoryMock = Substitute.For<IDatabaseConnectionServiceFactory>();
            databaseConnectionServiceFactoryMock.Create("connectionString").Returns(databaseConnectionServiceMock);

            loggingServiceMock = Substitute.For<ILoggingService>();

            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            systemUnderTest = NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock,
                databaseConnectionServiceFactoryMock, loggingServiceMock, errorMessageServiceMock);

            databaseConnectionServiceMock.CreateParameter(Arg.Any<string>(), Arg.Any<DbType>(),
                Arg.Any<ParameterDirection>()).Returns(info =>
                {
                    var parameterName = info.Arg<string>();
                    var dbType = info.Arg<DbType>();
                    var direction = info.Arg<ParameterDirection>();

                    var dbParameter = Substitute.For<DbParameter>();
                    dbParameter.ParameterName.Returns(parameterName);
                    dbParameter.DbType.Returns(dbType);
                    dbParameter.Direction.Returns(direction);

                    if (dbType.Equals(DbType.Int32))
                    {
                        dbParameter.Value.Returns(default(int));
                    }

                    return dbParameter;
                });

            databaseConnectionServiceMock.CreateParameter(Arg.Any<string>(), Arg.Any<DbType>(),
                Arg.Any<ParameterDirection>(), Arg.Any<int>()).Returns(info =>
                {
                    var parameterName = info.Arg<string>();
                    var dbType = info.Arg<DbType>();
                    var direction = info.Arg<ParameterDirection>();
                    var size = info.Arg<int>();

                    var dbParameter = Substitute.For<DbParameter>();
                    dbParameter.ParameterName.Returns(parameterName);
                    dbParameter.DbType.Returns(dbType);
                    dbParameter.Direction.Returns(direction);
                    dbParameter.Size.Returns(size);

                    if (dbType.Equals(DbType.Int32))
                    {
                        dbParameter.Value.Returns(default(int));
                    }

                    return dbParameter;
                });

            filePayload = new FilePayload
                          {
                              DownloadId = 100,
                              DecompressedDownloadFileName = "DecompressedDownloadFileName",
                              DecompressedDownloadFileExtension = "DecompressedDownloadFileExtension",
                              DecompressedDownloadFileData = "DecompressedDownloadFileData"
                          };

            fileDownloadResult = new FileDownloadResult(filePayload);

            dbDataReaderMock = Substitute.For<DbDataReader>();
            dbDataReaderMock.Read().Returns(true, true, true, false);
            dbDataReaderMock.GetInt32(0).Returns(100, 200, 300);

            databaseConnectionServiceMock.ExecuteReader("SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload]")
                                         .Returns(dbDataReaderMock);
        }

        [Test]
        public void StartStatsUpload_WhenInvoked_ParameterIsProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[StartStatsUpload]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            systemUnderTest.StartStatsUpload(100);

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(100));
        }

        [Test]
        public void StartStatsUpload_WhenInvoked_StartsStatsUpload()
        {
            systemUnderTest.StartStatsUpload(1);

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("StartStatsUpload Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[StartStatsUpload]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void StatsUploadError_WhenInvoked_ParametersAreProvided()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException).Returns("ErrorMessage");

            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[StatsUploadError]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            systemUnderTest.StatsUploadError(new StatsUploadResult(100, FailedReason.UnexpectedException));

            Assert.That(actualParameters.Count, Is.EqualTo(2));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(100));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@ErrorMessage"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo("ErrorMessage"));
        }

        [Test]
        public void StatsUploadError_WhenInvoked_UpdatesStatsUploadToError()
        {
            systemUnderTest.StatsUploadError(new StatsUploadResult());

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("StatsUploadError Invoked");
                databaseConnectionServiceMock.Open();
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[StatsUploadError]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            });
        }

        [Test]
        public void StatsUploadFinished_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[StatsUploadFinished]", Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            systemUnderTest.StatsUploadFinished(100);

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[0].Value, Is.EqualTo(100));
        }

        [Test]
        public void StatsUploadFinished_WhenInvoked_UpdatesStatsUploadToFinished()
        {
            systemUnderTest.StatsUploadFinished(100);

            Received.InOrder((() =>
            {
                loggingServiceMock.LogVerbose("StatsUploadFinished Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[StatsUploadFinished]",
                    Arg.Any<List<DbParameter>>());
                databaseConnectionServiceMock.Close();
            }));
        }

        [Test]
        public void UpdateToLatest_WhenInvoked_DatabaseUpdatedToLatest()
        {
            databaseConnectionServiceMock.ExecuteStoredProcedure(Arg.Any<string>())
                                         .Returns(NumberOfRowsEffectedExpected);

            InvokeUpdateToLatest();

            Received.InOrder((() =>
            {
                loggingServiceMock.LogVerbose("UpdateToLatest Invoked");
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[UpdateToLatest]");
                loggingServiceMock.LogVerbose($"'{NumberOfRowsEffectedExpected}' rows were effected");
                databaseConnectionServiceMock.Close();
            }));
        }

        private void InvokeFileDownloadError()
        {
            systemUnderTest.FileDownloadError(fileDownloadResult);
        }

        private void InvokeFileDownloadFinished()
        {
            systemUnderTest.FileDownloadFinished(filePayload);
        }

        private DateTime InvokeGetLastFileFownloadDateTime()
        {
            return systemUnderTest.GetLastFileDownloadDateTime();
        }

        private bool InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable();
        }

        private void InvokeNewFileDownloadStarted()
        {
            systemUnderTest.NewFileDownloadStarted(filePayload);
        }

        private void InvokeUpdateToLatest()
        {
            systemUnderTest.UpdateToLatest();
        }

        private IStatsDownloadDatabaseService NewFileDownloadDataStoreProvider(
            IDatabaseConnectionSettingsService databaseConnectionSettingsService,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory, ILoggingService loggingService,
            IErrorMessageService errorMessageService)
        {
            return new StatsDownloadDatabaseProvider(databaseConnectionSettingsService, databaseConnectionServiceFactory,
                loggingService, errorMessageService);
        }
    }
}