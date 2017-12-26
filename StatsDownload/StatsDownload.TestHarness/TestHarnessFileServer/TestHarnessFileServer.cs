namespace StatsDownload.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    public class TestHarnessFileServer : ITestHarnessFileServer
    {
        public Stream GetFile()
        {
            string filePath = ConfigurationManager.AppSettings["TestHarnessFileServer.FilePath"]
                              ?? Path.Combine(
                                  Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                  "MockFile",
                                  "TestHarnessStatsFile.txt.bz2");
            return new FileStream(filePath, FileMode.Open);
        }
    }
}