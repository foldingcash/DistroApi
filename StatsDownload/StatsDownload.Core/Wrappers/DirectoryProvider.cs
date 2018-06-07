namespace StatsDownload.Core.Wrappers
{
    using System.IO;
    using Interfaces;

    public class DirectoryProvider : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}