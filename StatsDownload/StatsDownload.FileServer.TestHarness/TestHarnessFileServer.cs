namespace StatsDownload.FileServer.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    public class TestHarnessFileServer : ITestHarnessFileServer
    {
        public Stream GetFile()
        {
            string filePath = GetFilePath();
            return new FileStream(filePath, FileMode.Open);
        }

        private string GetFilePath()
        {
            return ConfigurationManager.AppSettings["TestHarnessFileServer.FilePath"]
                   ?? Path.Combine(
                       Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                       "MockFile",
                       "TestHarnessStatsFile.txt.bz2");
        }
    }
}