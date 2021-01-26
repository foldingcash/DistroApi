namespace StatsDownload.Core.TestHarness
{
    using System.Collections.Concurrent;

    using Microsoft.Extensions.Logging;

    public class TestHarnessLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, TestHarnessLogger> loggerDictionary = new();

        public ILogger CreateLogger(string categoryName)
        {
            return loggerDictionary.GetOrAdd(categoryName, new TestHarnessLogger());
        }

        public void Dispose()
        {
            loggerDictionary.Clear();
        }
    }
}