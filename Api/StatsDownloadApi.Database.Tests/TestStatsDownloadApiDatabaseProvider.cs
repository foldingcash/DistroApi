﻿namespace StatsDownloadApi.Database.Tests
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
        public void GetDistroUsers_WhenInvoked_GetDistroUsersParametersAreProvided()
        {
            IEnumerable<DbParameter> actualParameters = null;

            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetDistroUsers]",
                    Arg.Any<IEnumerable<DbParameter>>(),
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                actualParameters = callInfo.Arg<IEnumerable<DbParameter>>();
            });

            systemUnderTest.GetDistroUsers(DateTime.MinValue, DateTime.MaxValue);

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
        public void GetDistroUsers_WhenInvoked_GetsDistroUsers()
        {
            databaseConnectionServiceMock.When(service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[GetDistroUsers]",
                    Arg.Any<IEnumerable<DbParameter>>(),
                    Arg.Any<DataTable>())).Do(callInfo =>
            {
                var dataTable = callInfo.Arg<DataTable>();
                dataTable.Columns.Add(new DataColumn("BitcoinAddress", typeof (string)));
                DataRow user1 = dataTable.NewRow();
                dataTable.Rows.Add(user1);
                user1["BitcoinAddress"] = "BitcoinAddress1";
                DataRow user2 = dataTable.NewRow();
                dataTable.Rows.Add(user2);
                user2["BitcoinAddress"] = "BitcoinAddress2";
                dataTable.AcceptChanges();
            });

            IList<DistroUser> actual = systemUnderTest.GetDistroUsers(DateTime.MinValue, DateTime.MaxValue);

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("BitcoinAddress1"));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("BitcoinAddress2"));
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