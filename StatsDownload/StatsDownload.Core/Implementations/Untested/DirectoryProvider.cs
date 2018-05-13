namespace StatsDownload.Core.Implementations.Untested
{
    using System.IO;

    using StatsDownload.Core.Interfaces;

    public class DirectoryProvider : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}