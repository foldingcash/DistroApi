namespace StatsDownload.Core.TestHarness
{
    using System;

    using Microsoft.Extensions.Logging;

    public class TestHarnessLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(formatter(state, exception));
        }
    }
}