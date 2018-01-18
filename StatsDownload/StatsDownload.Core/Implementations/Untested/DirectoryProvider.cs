namespace StatsDownload.Core
{
    using System.IO;

    public class DirectoryProvider : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}