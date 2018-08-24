namespace StatsDownloadApi.Interfaces
{
    using System;

    public interface IStatsDownloadApiEmailService
    {
        void SendUnhandledExceptionEmail(Exception exception);
    }
}