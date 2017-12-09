namespace StatsDownload.Core
{
    using System;

    public interface IDatabaseConnectionService : IDisposable
    {
        void Close();

        void Open();
    }
}