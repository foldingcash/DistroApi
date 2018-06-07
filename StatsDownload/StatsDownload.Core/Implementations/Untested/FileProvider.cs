namespace StatsDownload.Core.Implementations.Untested
{
    using System.IO;
    using Interfaces;

    public class FileProvider : IFileService
    {
        public void CopyFile(string source, string target)
        {
            EnsureFileDirectoryExists(target);
            File.Copy(source, target, true);
        }

        public void CreateFromStream(string path, Stream source)
        {
            EnsureFileDirectoryExists(path);

            using (FileStream target = File.Create(path))
            {
                source.CopyTo(target);
                target.Flush();
            }
        }

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
            EnsureFileDirectoryExists(destinationPath);

            File.Move(sourcePath, destinationPath);
        }

        private void EnsureFileDirectoryExists(string path)
        {
            string destinationDirectory = Path.GetDirectoryName(path);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
        }
    }
}