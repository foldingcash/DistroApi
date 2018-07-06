namespace StatsDownload.Database.Tests
{
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
    using NSubstitute;
    using NSubstitute.Core;

    internal static class TestDatabaseProviderHelper
    {
        internal static void SetUpDatabaseConnectionServiceReturns(
            IDatabaseConnectionService databaseConnectionServiceMock)
        {
            databaseConnectionServiceMock.CreateParameter(Arg.Any<string>(), Arg.Any<DbType>(),
                Arg.Any<ParameterDirection>()).Returns(CreateParameterMock);

            databaseConnectionServiceMock.CreateParameter(Arg.Any<string>(), Arg.Any<DbType>(),
                                             Arg.Any<ParameterDirection>(), Arg.Any<int>())
                                         .Returns(CreateParameterMockWithSize);
        }

        private static DbParameter CreateParameterMock(CallInfo info)
        {
            var parameterName = info.Arg<string>();
            var dbType = info.Arg<DbType>();
            var direction = info.Arg<ParameterDirection>();

            var parameterMock = Substitute.For<DbParameter>();
            parameterMock.ParameterName.Returns(parameterName);
            parameterMock.DbType.Returns(dbType);
            parameterMock.Direction.Returns(direction);

            if (dbType.Equals(DbType.Int32))
            {
                parameterMock.Value.Returns(default(int));
            }

            return parameterMock;
        }

        private static DbParameter CreateParameterMockWithSize(CallInfo info)
        {
            DbParameter parameterMock = CreateParameterMock(info);
            var size = info.Arg<int>();
            parameterMock.Size.Returns(size);
            return parameterMock;
        }
    }
}