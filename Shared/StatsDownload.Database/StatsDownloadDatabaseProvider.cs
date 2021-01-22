﻿namespace StatsDownload.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Settings;

    public class StatsDownloadDatabaseProvider : IStatsDownloadDatabaseService
    {
        private const string DatabaseConnectionSuccessfulLogMessage = "Database connection was successful";

        private readonly IDatabaseConnectionServiceFactory databaseConnectionServiceFactory;

        private readonly DatabaseSettings databaseSettings;

        private readonly ILoggingService loggingService;

        public StatsDownloadDatabaseProvider(IOptions<DatabaseSettings> databaseSettings,
                                             IDatabaseConnectionServiceFactory databaseConnectionServiceFactory,
                                             ILoggingService loggingService)
        {
            this.databaseSettings = databaseSettings?.Value
                                    ?? throw new ArgumentNullException(nameof(databaseSettings));
            this.databaseConnectionServiceFactory = databaseConnectionServiceFactory
                                                    ?? throw new ArgumentNullException(
                                                        nameof(databaseConnectionServiceFactory));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public void Commit(DbTransaction transaction)
        {
            transaction?.Commit();
        }

        public void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            string connectionString = databaseSettings.ConnectionString;
            EnsureValidConnectionString(connectionString);
            IDatabaseConnectionService databaseConnection = CreateDatabaseConnection();
            EnsureDatabaseConnectionOpened(databaseConnection);
            action?.Invoke(databaseConnection);
        }

        public DbTransaction CreateTransaction()
        {
            loggingService.LogMethodInvoked();
            DbTransaction transaction = null;
            CreateDatabaseConnectionAndExecuteAction(service => { transaction = CreateTransaction(service); });
            return transaction;
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable(string[] requiredObjects)
        {
            loggingService.LogMethodInvoked();

            try
            {
                var failedReason = DatabaseFailedReason.None;

                CreateDatabaseConnectionAndExecuteAction(service =>
                {
                    var missingObjects = new List<string>();

                    foreach (string requiredObject in requiredObjects ?? new string[0])
                    {
                        object objectId = service.ExecuteScalar($"SELECT OBJECT_ID('{requiredObject}')");

                        if (objectId == DBNull.Value)
                        {
                            missingObjects.Add(requiredObject);
                        }
                    }

                    if (missingObjects.Count > 0)
                    {
                        string missingObjectsCombined = "{'" + string.Join("', '", missingObjects) + "'}";

                        loggingService.LogError(
                            $"The required objects {missingObjectsCombined} are missing from the database.");

                        failedReason = DatabaseFailedReason.DatabaseMissingRequiredObjects;
                    }
                });

                return (failedReason == DatabaseFailedReason.None, failedReason);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return (false, DatabaseFailedReason.DatabaseUnavailable);
            }
        }

        public void Rollback(DbTransaction transaction)
        {
            loggingService.LogMethodInvoked();
            transaction?.Rollback();
        }

        private IDatabaseConnectionService CreateDatabaseConnection()
        {
            return databaseConnectionServiceFactory.Create();
        }

        private DbTransaction CreateTransaction(IDatabaseConnectionService service)
        {
            return service.CreateTransaction();
        }

        private void EnsureDatabaseConnectionOpened(IDatabaseConnectionService databaseConnection)
        {
            if (databaseConnection.ConnectionState == ConnectionState.Closed)
            {
                databaseConnection.Open();
                LogVerbose(DatabaseConnectionSuccessfulLogMessage);
            }
        }

        private void EnsureValidConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Argument is empty", nameof(connectionString));
            }
        }

        private void LogException(Exception exception)
        {
            loggingService.LogException(exception);
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }
    }
}