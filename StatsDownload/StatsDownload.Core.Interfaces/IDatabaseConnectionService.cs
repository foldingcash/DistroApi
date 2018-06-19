namespace StatsDownload.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    public interface IDatabaseConnectionService : IDisposable
    {
        ConnectionState ConnectionState { get; }

        void Close();

        DbCommand CreateDbCommand();

        DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction);

        DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction, int size);

        DbTransaction CreateTransaction();

        DbDataReader ExecuteReader(string commandText);

        object ExecuteScalar(string commandText);

        int ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters);

        int ExecuteStoredProcedure(DbTransaction transaction, string storedProcedure,
            IEnumerable<DbParameter> parameters);

        int ExecuteStoredProcedure(string storedProcedure);

        void Open();
    }
}