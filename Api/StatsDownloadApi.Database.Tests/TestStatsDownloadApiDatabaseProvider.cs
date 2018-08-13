namespace StatsDownloadApi.Database.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;
    using NSubstitute;
    using NUnit.Framework;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Database.Tests;

    [TestFixture]
    public class TestStatsDownloadApiDatabaseProvider
    {
        [SetUp]
        public void SetUp()
        {
            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();

            statsDownloadDatabaseServiceMock = Substitute.For<IStatsDownloadDatabaseService>();
            statsDownloadDatabaseServiceMock.When(service =>
                service.CreateDatabaseConnectionAndExecuteAction(
                    Arg.Any<Action<IDatabaseConnectionService>>())).Do(
                callInfo =>
                {
                    var service = callInfo.Arg<Action<IDatabaseConnectionService>>();

                    service.Invoke(databaseConnectionServiceMock);
                });

            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);

            systemUnderTest = NewStatsDownloadApiDatabaseProvider(statsDownloadDatabaseServiceMock);
        }

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private IStatsDownloadDatabaseService statsDownloadDatabaseServiceMock;

        private IStatsDownloadApiDatabaseService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewStatsDownloadApiDatabaseProvider(null));
        }

        [Test]
        public void GetFoldingMembers_WhenInvoked_GetFoldingMembersParametersAreProvided()
        {
            IEnumerable<DbParameter> actualParameters = null;

            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetFoldingMembers]",
                    Arg.Any<IEnumerable<DbParameter>>(),
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                actualParameters = callInfo.Arg<IEnumerable<DbParameter>>();
            });

            systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue);

            Assert.That(actualParameters.Count, Is.EqualTo(2));
            Assert.That(actualParameters.ElementAt(0).ParameterName, Is.EqualTo("@StartDate"));
            Assert.That(actualParameters.ElementAt(0).DbType, Is.EqualTo(DbType.Date));
            Assert.That(actualParameters.ElementAt(0).Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters.ElementAt(0).Value, Is.EqualTo(DateTime.MinValue));
            Assert.That(actualParameters.ElementAt(1).ParameterName, Is.EqualTo("@EndDate"));
            Assert.That(actualParameters.ElementAt(1).DbType, Is.EqualTo(DbType.Date));
            Assert.That(actualParameters.ElementAt(1).Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters.ElementAt(1).Value, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void GetFoldingMembers_WhenInvoked_GetsFoldingUsers()
        {
            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetFoldingMembers]",
                    Arg.Any<IEnumerable<DbParameter>>(),
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                var dataTable = callInfo.Arg<DataTable>();
                dataTable.Columns.Add(new DataColumn("FriendlyName", typeof (string)));
                dataTable.Columns.Add(new DataColumn("BitcoinAddress", typeof (string)));
                dataTable.Columns.Add(new DataColumn("PointsGained", typeof (long)));
                dataTable.Columns.Add(new DataColumn("WorkUnitsGained", typeof (long)));
                DataRow user1 = dataTable.NewRow();
                dataTable.Rows.Add(user1);
                user1["FriendlyName"] = "FriendlyName1";
                user1["BitcoinAddress"] = "BitcoinAddress1";
                user1["PointsGained"] = 100;
                user1["WorkUnitsGained"] = 1000;
                DataRow user2 = dataTable.NewRow();
                dataTable.Rows.Add(user2);
                user2["FriendlyName"] = "FriendlyName2";
                user2["BitcoinAddress"] = "BitcoinAddress2";
                user2["PointsGained"] = 200;
                user2["WorkUnitsGained"] = 2000;
                dataTable.AcceptChanges();
            });

            IList<FoldingUser> actual = systemUnderTest.GetFoldingMembers(DateTime.MinValue, DateTime.MaxValue);

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].FriendlyName, Is.EqualTo("FriendlyName1"));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("BitcoinAddress1"));
            Assert.That(actual[0].PointsGained, Is.EqualTo(100));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(1000));
            Assert.That(actual[1].FriendlyName, Is.EqualTo("FriendlyName2"));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("BitcoinAddress2"));
            Assert.That(actual[1].PointsGained, Is.EqualTo(200));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(2000));
        }

        [Test]
        public void GetMembers_WhenInvoked_GetMembersParametersAreProvided()
        {
            IEnumerable<DbParameter> actualParameters = null;

            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetMembers]",
                    Arg.Any<IEnumerable<DbParameter>>(),
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                actualParameters = callInfo.Arg<IEnumerable<DbParameter>>();
            });

            systemUnderTest.GetMembers(DateTime.MinValue, DateTime.MaxValue);

            Assert.That(actualParameters.Count, Is.EqualTo(2));
            Assert.That(actualParameters.ElementAt(0).ParameterName, Is.EqualTo("@StartDate"));
            Assert.That(actualParameters.ElementAt(0).DbType, Is.EqualTo(DbType.Date));
            Assert.That(actualParameters.ElementAt(0).Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters.ElementAt(0).Value, Is.EqualTo(DateTime.MinValue));
            Assert.That(actualParameters.ElementAt(1).ParameterName, Is.EqualTo("@EndDate"));
            Assert.That(actualParameters.ElementAt(1).DbType, Is.EqualTo(DbType.Date));
            Assert.That(actualParameters.ElementAt(1).Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters.ElementAt(1).Value, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void GetMembers_WhenInvoked_GetsMembers()
        {
            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetMembers]",
                    Arg.Any<IEnumerable<DbParameter>>(),
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                var dataTable = callInfo.Arg<DataTable>();
                dataTable.Columns.Add(new DataColumn("UserName", typeof (string)));
                dataTable.Columns.Add(new DataColumn("FriendlyName", typeof (string)));
                dataTable.Columns.Add(new DataColumn("BitcoinAddress", typeof (string)));
                dataTable.Columns.Add(new DataColumn("TeamNumber", typeof (long)));
                dataTable.Columns.Add(new DataColumn("PointsGained", typeof (long)));
                dataTable.Columns.Add(new DataColumn("WorkUnitsGained", typeof (long)));
                DataRow user1 = dataTable.NewRow();
                dataTable.Rows.Add(user1);
                user1["UserName"] = "Name1";
                user1["FriendlyName"] = "FriendlyName1";
                user1["BitcoinAddress"] = "BitcoinAddress1";
                user1["TeamNumber"] = 10;
                user1["PointsGained"] = 100;
                user1["WorkUnitsGained"] = 1000;
                DataRow user2 = dataTable.NewRow();
                dataTable.Rows.Add(user2);
                user2["UserName"] = "Name2";
                user2["FriendlyName"] = "FriendlyName2";
                user2["BitcoinAddress"] = "BitcoinAddress2";
                user2["TeamNumber"] = 20;
                user2["PointsGained"] = 200;
                user2["WorkUnitsGained"] = 2000;
                dataTable.AcceptChanges();
            });

            IList<Member> actual = systemUnderTest.GetMembers(DateTime.MinValue, DateTime.MaxValue);

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].UserName, Is.EqualTo("Name1"));
            Assert.That(actual[0].FriendlyName, Is.EqualTo("FriendlyName1"));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("BitcoinAddress1"));
            Assert.That(actual[0].TeamNumber, Is.EqualTo(10));
            Assert.That(actual[0].PointsGained, Is.EqualTo(100));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(1000));
            Assert.That(actual[1].UserName, Is.EqualTo("Name2"));
            Assert.That(actual[1].FriendlyName, Is.EqualTo("FriendlyName2"));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("BitcoinAddress2"));
            Assert.That(actual[1].TeamNumber, Is.EqualTo(20));
            Assert.That(actual[1].PointsGained, Is.EqualTo(200));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(2000));
        }

        [Test]
        public void GetTeams_WhenInvoked_GetsTeams()
        {
            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetTeams]",
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                var dataTable = callInfo.Arg<DataTable>();
                dataTable.Columns.Add(new DataColumn("TeamNumber", typeof (long)));
                dataTable.Columns.Add(new DataColumn("TeamName", typeof (string)));
                DataRow user1 = dataTable.NewRow();
                dataTable.Rows.Add(user1);
                user1["TeamNumber"] = 100;
                user1["TeamName"] = "TeamName1";
                DataRow user2 = dataTable.NewRow();
                dataTable.Rows.Add(user2);
                user2["TeamNumber"] = 200;
                user2["TeamName"] = "TeamName2";
                dataTable.AcceptChanges();
            });

            IList<Team> actual = systemUnderTest.GetTeams();

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].TeamNumber, Is.EqualTo(100));
            Assert.That(actual[0].TeamName, Is.EqualTo("TeamName1"));
            Assert.That(actual[1].TeamNumber, Is.EqualTo(200));
            Assert.That(actual[1].TeamName, Is.EqualTo("TeamName2"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsAvailable_WhenInvoked_ReturnsIsAvailable(bool expected)
        {
            statsDownloadDatabaseServiceMock.IsAvailable().Returns(expected);

            bool actual = systemUnderTest.IsAvailable();

            Assert.That(actual, Is.EqualTo(expected));
        }

        private IStatsDownloadApiDatabaseService NewStatsDownloadApiDatabaseProvider(
            IStatsDownloadDatabaseService statsDownloadDatabaseService)
        {
            return new StatsDownloadApiDatabaseProvider(statsDownloadDatabaseService);
        }
    }
}