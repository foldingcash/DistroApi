namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.Collections.Concurrent;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces.Logging;

    public class TestHarnessLoggingService : IApplicationLoggingService
    {
        private readonly ILogger logger;

        public TestHarnessLoggingService(ILogger<TestHarnessLoggingService> logger)
        {
            this.logger = logger;
        }

        public void LogError(string message)
        {
            logger.LogError(message);
        }

        public void LogVerbose(string message)
        {
            logger.LogDebug(message);
        }
    }

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

    public class TestHarnessLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, TestHarnessLogger> loggers =
            new ConcurrentDictionary<string, TestHarnessLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, new TestHarnessLogger());
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}