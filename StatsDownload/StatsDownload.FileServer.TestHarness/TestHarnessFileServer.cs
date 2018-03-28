namespace StatsDownload.FileServer.TestHarness
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public class TestHarnessFileServer : ITestHarnessFileServer
    {
        private const string GoodStatsFile = "TestHarnessStatsFile.txt.bz2";

        private const string InvalidFolderRecordFile = "InvalidFolderRecord.txt.bz2";

        private const string InvalidStatsFile = "InvalidStatsFile.txt.bz2";

        public Stream GetDecompressableFile()
        {
            var memoryStream = new MemoryStream();
            const string Decompressable = "decompressable";
            byte[] decompressableBytes = Encoding.ASCII.GetBytes(Decompressable);
            memoryStream.Write(decompressableBytes, 0, decompressableBytes.Length);
            memoryStream.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        public Stream GetFailDownloadFile()
        {
            throw new NotImplementedException();
        }

        public Stream GetFile()
        {
            return GetFileStream(GoodStatsFile);
        }

        public Stream GetInvalidFolderRecord()
        {
            return GetFileStream(InvalidFolderRecordFile);
        }

        public Stream GetInvalidStatsFile()
        {
            return GetFileStream(InvalidStatsFile);
        }

        public Stream GetTimeoutFile()
        {
            int sleepInSeconds = GetSleepInSeconds();
            Thread.Sleep(sleepInSeconds * 1000);
            return GetFile();
        }

        private string GetFilePath(string fileName)
        {
            return ConfigurationManager.AppSettings["FilePath"]
                   ?? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MockFiles",
                       fileName);
        }

        private Stream GetFileStream(string fileName)
        {
            string filePath = GetFilePath(fileName);
            return new FileStream(filePath, FileMode.Open);
        }

        private int GetSleepInSeconds()
        {
            int sleepInSeconds;
            int.TryParse(ConfigurationManager.AppSettings["SleepInSeconds"], out sleepInSeconds);
            return sleepInSeconds == 0 ? 100 : sleepInSeconds;
        }
    }
}