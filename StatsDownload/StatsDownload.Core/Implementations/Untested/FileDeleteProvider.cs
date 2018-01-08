namespace StatsDownload.Core
{
    using System.IO;

    public class FileDeleteProvider : IFileDeleteService
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}