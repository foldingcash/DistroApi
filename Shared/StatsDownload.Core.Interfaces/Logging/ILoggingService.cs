namespace StatsDownload.Core.Interfaces.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    public interface ILoggingService
    {
        void LogDebug(string message);

        void LogError(string message);

        void LogException(Exception exception);

        void LogMethodFinished([CallerMemberName] string method = "");

        void LogMethodInvoked([CallerMemberName] string method = "");
    }
}