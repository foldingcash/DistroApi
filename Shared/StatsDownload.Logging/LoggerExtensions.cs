namespace StatsDownload.Logging
{
    using System.Runtime.CompilerServices;

    using Microsoft.Extensions.Logging;

    public static class LoggerExtensions
    {
        public static void LogMethodFinished(this ILogger logger, [CallerMemberName] string method = "")
        {
            logger.LogDebug("{method} Finished", method);
        }

        public static void LogMethodInvoked(this ILogger logger, [CallerMemberName] string method = "")
        {
            logger.LogDebug("{method} Invoked", method);
        }
    }
}