namespace StatsDownload.Core.Implementations.Untested
{
    using System.IO;

    using StatsDownload.Core.Interfaces;

    public class FileProvider : IFileService
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Move(string sourcePath, string destinationPath)
        {
            string destinationDirectory = Path.GetDirectoryName(destinationPath);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Move(sourcePath, destinationPath);
        }
    }
}