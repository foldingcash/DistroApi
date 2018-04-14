﻿namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    public interface IDatabaseConnectionService : IDisposable
    {
        void Close();

        DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction);

        DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction, int size);

        DbDataReader ExecuteReader(string commandText);

        object ExecuteScalar(string commandText);

        int ExecuteStoredProcedure(string storedProcedure, List<DbParameter> parameters);

        int ExecuteStoredProcedure(string storedProcedure);

        void Open();
    }
}