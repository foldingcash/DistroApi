namespace StatsDownload.FileServer.TestHarness
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class TestHarnessFileServer : ITestHarnessFileServer
    {
        public Stream GetFailDownloadFile()
        {
            throw new NotImplementedException();
        }

        public Stream GetFile()
        {
            string filePath = GetFilePath();
            return new FileStream(filePath, FileMode.Open);
        }

        public Stream GetUncompressableFile()
        {
            var memoryStream = new MemoryStream();
            const string Uncompressable = "uncompressable";
            byte[] uncompressableBytes = Encoding.ASCII.GetBytes(Uncompressable);
            memoryStream.Write(uncompressableBytes, 0, uncompressableBytes.Length);
            memoryStream.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private string GetFilePath()
        {
            return ConfigurationManager.AppSettings["FilePath"]
                   ?? Path.Combine(
                       Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                       "MockFile",
                       "TestHarnessStatsFile.txt.bz2");
        }
    }
}