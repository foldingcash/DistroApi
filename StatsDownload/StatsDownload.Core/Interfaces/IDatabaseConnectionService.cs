namespace StatsDownload.Core
{
    using System;

    public interface IDatabaseConnectionService : IDisposable
    {
        void ExecuteStoredProcedure();

        void Open();
    }
}