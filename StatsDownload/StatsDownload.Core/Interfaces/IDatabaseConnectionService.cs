namespace StatsDownload.Core
{
    using System;

    public interface IDatabaseConnectionService : IDisposable
    {
        void Close();

        int ExecuteStoredProcedure(string storedProcedure);

        void Open();
    }
}